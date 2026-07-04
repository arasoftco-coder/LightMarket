using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LampEcommerce.Application.Services;

public class CartService : ICartService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICampaignRepository _campaignRepository;
    private readonly ICampaignProductRepository _campaignProductRepository;
    private readonly IProductRepository _productRepository;

    public CartService(
        IOrderRepository orderRepository,
        ICampaignRepository campaignRepository,
        ICampaignProductRepository campaignProductRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _campaignRepository = campaignRepository;
        _campaignProductRepository = campaignProductRepository;
        _productRepository = productRepository;
    }

    public async Task<CartItemDto?> AddToCart(int userId, int campaignId, int productId, int quantity)
    {
        var campaign = await _campaignRepository.GetByIdAsync(campaignId);
        if (campaign == null || !campaign.IsActive || campaign.EndDate < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Campaign is not active.");
        }

        var campaignProduct = await _campaignProductRepository.GetCampaignProductAsync(campaignId, productId);
        if (campaignProduct == null || campaignProduct.Stock < quantity)
        {
            throw new InvalidOperationException("Product is out of stock or requested quantity exceeds available stock.");
        }

        // Find or create "Open" order (Cart)
        var orders = await _orderRepository.GetByUserIdAsync(userId);
        var cartOrder = orders.FirstOrDefault(o => o.CampaignId == campaignId && o.Status == "Open");

        if (cartOrder == null)
        {
            cartOrder = new Order
            {
                UserId = userId,
                CampaignId = campaignId,
                Status = "Open",
                ShippingMethod = "Standard",
                PaymentMethod = "Online",
                CreatedAt = DateTime.UtcNow
            };
            cartOrder = await _orderRepository.AddAsync(cartOrder);
        }

        var orderItem = cartOrder.OrderItems.FirstOrDefault(oi => oi.ProductId == productId);
        if (orderItem == null)
        {
            orderItem = new OrderItem
            {
                OrderId = cartOrder.Id,
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = campaignProduct.SellingPrice - campaignProduct.Discount,
                TotalPrice = (campaignProduct.SellingPrice - campaignProduct.Discount) * quantity
            };
            cartOrder.OrderItems.Add(orderItem);
        }
        else
        {
            orderItem.Quantity += quantity;
            orderItem.TotalPrice = orderItem.UnitPrice * orderItem.Quantity;
        }

        RecalculateTotals(cartOrder);
        await _orderRepository.UpdateAsync(cartOrder);

        var product = await _productRepository.GetByIdAsync(productId);

        return new CartItemDto
        {
            Id = orderItem.Id,
            UserId = userId,
            ProductId = productId,
            Quantity = orderItem.Quantity,
            UnitPrice = orderItem.UnitPrice,
            AddedAt = DateTime.UtcNow,
            Product = product == null ? null : new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                ImageUrl = product.ImageUrl,
                Description = product.Description,
                BasePrice = product.BasePrice
            }
        };
    }

    public async Task<CartItemDto?> UpdateQuantity(int userId, int cartItemId, int quantity)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId);
        var cartOrder = orders.FirstOrDefault(o => o.OrderItems.Any(oi => oi.Id == cartItemId) && o.Status == "Open");
        if (cartOrder == null) return null;

        var orderItem = cartOrder.OrderItems.First(oi => oi.Id == cartItemId);

        var campaignProduct = await _campaignProductRepository.GetCampaignProductAsync(cartOrder.CampaignId, orderItem.ProductId);
        if (campaignProduct == null || campaignProduct.Stock < quantity)
        {
            throw new InvalidOperationException("Not enough stock available.");
        }

        orderItem.Quantity = quantity;
        orderItem.TotalPrice = orderItem.UnitPrice * quantity;

        RecalculateTotals(cartOrder);
        await _orderRepository.UpdateAsync(cartOrder);

        var product = await _productRepository.GetByIdAsync(orderItem.ProductId);

        return new CartItemDto
        {
            Id = orderItem.Id,
            UserId = userId,
            ProductId = orderItem.ProductId,
            Quantity = orderItem.Quantity,
            UnitPrice = orderItem.UnitPrice,
            AddedAt = DateTime.UtcNow,
            Product = product == null ? null : new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                ImageUrl = product.ImageUrl,
                Description = product.Description,
                BasePrice = product.BasePrice
            }
        };
    }

    public async Task<bool> RemoveFromCart(int userId, int cartItemId)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId);
        var cartOrder = orders.FirstOrDefault(o => o.OrderItems.Any(oi => oi.Id == cartItemId) && o.Status == "Open");
        if (cartOrder == null) return false;

        await _orderRepository.DeleteOrderItemAsync(cartItemId);
        
        // Load again and recalculate totals
        var updatedOrders = await _orderRepository.GetByUserIdAsync(userId);
        var updatedCartOrder = updatedOrders.FirstOrDefault(o => o.Id == cartOrder.Id);
        if (updatedCartOrder != null)
        {
            RecalculateTotals(updatedCartOrder);
            await _orderRepository.UpdateAsync(updatedCartOrder);
        }

        return true;
    }

    public async Task<CartDto?> GetCart(int userId, int campaignId)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId);
        var cartOrder = orders.FirstOrDefault(o => o.CampaignId == campaignId && o.Status == "Open");

        if (cartOrder == null)
        {
            return new CartDto
            {
                UserId = userId,
                Items = new List<CartItemDto>()
            };
        }

        return new CartDto
        {
            UserId = userId,
            Items = cartOrder.OrderItems.Select(oi => new CartItemDto
            {
                Id = oi.Id,
                UserId = userId,
                ProductId = oi.ProductId,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                AddedAt = cartOrder.CreatedAt,
                Product = oi.Product == null ? null : new ProductDto
                {
                    Id = oi.Product.Id,
                    Name = oi.Product.Name,
                    ImageUrl = oi.Product.ImageUrl,
                    Description = oi.Product.Description,
                    BasePrice = oi.Product.BasePrice
                }
            }).ToList()
        };
    }

    private static void RecalculateTotals(Order order)
    {
        order.TotalAmount = order.OrderItems.Sum(oi => oi.TotalPrice) + order.ShippingCost + order.TaxAmount - order.DiscountAmount;
    }
}
