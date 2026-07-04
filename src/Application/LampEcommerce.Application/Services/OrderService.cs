using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LampEcommerce.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICampaignProductRepository _campaignProductRepository;
    private readonly IUserRepository _userRepository;

    public OrderService(
        IOrderRepository orderRepository,
        ICampaignProductRepository campaignProductRepository,
        IUserRepository userRepository)
    {
        _orderRepository = orderRepository;
        _campaignProductRepository = campaignProductRepository;
        _userRepository = userRepository;
    }

    public async Task<OrderDto?> CreateOrder(int userId, int cartId, int addressId, string shippingMethod, string paymentMethod)
    {
        var order = await _orderRepository.GetByIdWithDetailsAsync(cartId);
        if (order == null || order.UserId != userId || order.Status != "Open" || !order.OrderItems.Any())
        {
            throw new InvalidOperationException("Cart is empty or not found.");
        }

        // Validate address exists on user
        var user = await _userRepository.GetByIdWithDetailsAsync(userId);
        var address = user?.Addresses.FirstOrDefault(a => a.Id == addressId);
        if (address == null)
        {
            throw new InvalidOperationException("Delivery address not found.");
        }

        // Hold stock and validate quantities
        foreach (var item in order.OrderItems)
        {
            var campaignProduct = await _campaignProductRepository.GetCampaignProductAsync(order.CampaignId, item.ProductId);
            if (campaignProduct == null || campaignProduct.Stock < item.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock for product {item.ProductId}.");
            }

            campaignProduct.Stock -= item.Quantity;
            await _campaignProductRepository.UpdateAsync(campaignProduct);
        }

        // Set costs
        order.ShippingMethod = shippingMethod;
        order.PaymentMethod = paymentMethod;
        order.ShippingCost = 30000; // Flat rate for shipping
        order.TaxAmount = Math.Round(order.OrderItems.Sum(oi => oi.TotalPrice) * 0.09m, 2); // 9% VAT
        order.Status = "PaymentPending";
        order.CreatedAt = DateTime.UtcNow;

        RecalculateTotals(order);
        await _orderRepository.UpdateAsync(order);

        return await GetOrderDetails(order.Id);
    }

    public async Task<OrderDto?> ConfirmPayment(int orderId, string trackingCode)
    {
        var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);
        if (order == null) return null;

        order.Status = "PaymentConfirmed";
        order.PaymentTrackingCode = trackingCode;

        await _orderRepository.UpdateAsync(order);

        return await GetOrderDetails(orderId);
    }

    public async Task<OrderDto?> EditInvoice(int orderId, Dictionary<string, object> changes, string reason, string changedBy)
    {
        var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);
        if (order == null) return null;

        var prevData = System.Text.Json.JsonSerializer.Serialize(new
        {
            order.TotalAmount,
            order.DiscountAmount,
            order.TaxAmount,
            order.ShippingCost,
            Items = order.OrderItems.Select(oi => new { oi.ProductId, oi.Quantity, oi.UnitPrice })
        });

        // Apply changes
        if (changes.TryGetValue("ShippingCost", out var shipCostObj) && decimal.TryParse(shipCostObj.ToString(), out var shipCost))
        {
            order.ShippingCost = shipCost;
        }
        if (changes.TryGetValue("DiscountAmount", out var discObj) && decimal.TryParse(discObj.ToString(), out var disc))
        {
            order.DiscountAmount = disc;
        }

        RecalculateTotals(order);

        var newData = System.Text.Json.JsonSerializer.Serialize(new
        {
            order.TotalAmount,
            order.DiscountAmount,
            order.TaxAmount,
            order.ShippingCost,
            Items = order.OrderItems.Select(oi => new { oi.ProductId, oi.Quantity, oi.UnitPrice })
        });

        var auditLog = new InvoiceAuditLog
        {
            OrderId = orderId,
            PreviousInvoiceData = prevData,
            NewInvoiceData = newData,
            ChangedBy = changedBy,
            ChangedAt = DateTime.UtcNow,
            Reason = reason
        };

        order.InvoiceAuditLogs.Add(auditLog);
        await _orderRepository.UpdateAsync(order);

        return await GetOrderDetails(orderId);
    }

    public async Task<OrderDto?> UpdateShippingInfo(int orderId, string trackingCode, string shippingCompany)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null) return null;

        order.Status = "Shipped";
        order.PaymentTrackingCode = trackingCode; // Storing tracking number in tracking code field
        order.ShippingMethod = shippingCompany;

        await _orderRepository.UpdateAsync(order);

        return await GetOrderDetails(orderId);
    }

    public async Task<IEnumerable<OrderDto>> GetUserOrders(int userId)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId);
        return orders.Where(o => o.Status != "Open").Select(MapToDto);
    }

    public async Task<OrderDto?> GetOrderDetails(int orderId)
    {
        var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);
        if (order == null) return null;

        return MapToDto(order);
    }

    private static void RecalculateTotals(Order order)
    {
        order.TotalAmount = order.OrderItems.Sum(oi => oi.TotalPrice) + order.ShippingCost + order.TaxAmount - order.DiscountAmount;
    }

    private static OrderDto MapToDto(Order o)
    {
        return new OrderDto
        {
            Id = o.Id,
            UserId = o.UserId,
            CampaignId = o.CampaignId,
            TotalAmount = o.TotalAmount,
            DiscountAmount = o.DiscountAmount,
            TaxAmount = o.TaxAmount,
            ShippingCost = o.ShippingCost,
            ShippingMethod = o.ShippingMethod,
            PaymentMethod = o.PaymentMethod,
            PaymentTrackingCode = o.PaymentTrackingCode,
            Status = o.Status,
            CreatedAt = o.CreatedAt,
            User = o.User == null ? null : new UserDto
            {
                Id = o.User.Id,
                FullName = o.User.FullName,
                PhoneNumber = o.User.PhoneNumber,
                Role = o.User.Role,
                Avatar = o.User.Avatar,
                CreatedAt = o.User.CreatedAt
            },
            Campaign = o.Campaign == null ? null : new CampaignDto
            {
                Id = o.Campaign.Id,
                Name = o.Campaign.Name,
                Slug = o.Campaign.Slug,
                SupplierId = o.Campaign.SupplierId,
                StartDate = o.Campaign.StartDate,
                EndDate = o.Campaign.EndDate,
                IsActive = o.Campaign.IsActive,
                IsDefault = o.Campaign.IsDefault
            },
            OrderItems = o.OrderItems.Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                OrderId = oi.OrderId,
                ProductId = oi.ProductId,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                TotalPrice = oi.TotalPrice,
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
}
