using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Application.Models;

namespace LampEcommerce.Application.Services;

public class OrderService : IOrderService
{
    public Task<OrderDto?> CreateOrder(int userId, int cartId, int addressId, string shippingMethod, string paymentMethod)
    {
        // Calculate totals (subtotal, discount, tax, shipping)
        // Create order with status "PaymentPending"
        // Reduce stock temporarily (hold stock)
        
        var order = new OrderDto
        {
            Id = 1,
            UserId = userId,
            CampaignId = 1,
            TotalAmount = 500.00m,
            DiscountAmount = 50.00m,
            TaxAmount = 45.00m,
            ShippingCost = 30.00m,
            ShippingMethod = shippingMethod,
            PaymentMethod = paymentMethod,
            Status = "PaymentPending",
            CreatedAt = DateTime.Now,
            OrderItems = new List<OrderItemDto>
            {
                new OrderItemDto
                {
                    Id = 1,
                    ProductId = 1,
                    Quantity = 2,
                    UnitPrice = 150.00m,
                    TotalPrice = 300.00m
                }
            }
        };
        return Task.FromResult<OrderDto?>(order);
    }

    public Task<OrderDto?> ConfirmPayment(int orderId, string trackingCode)
    {
        // Change status to "PaymentConfirmed"
        // Send SMS to admin and user
        // LOCK: Cannot proceed to supplier until this is called
        
        var order = new OrderDto
        {
            Id = orderId,
            Status = "PaymentConfirmed",
            PaymentTrackingCode = trackingCode,
            TotalAmount = 500.00m,
            CreatedAt = DateTime.Now
        };
        return Task.FromResult<OrderDto?>(order);
    }

    public Task<OrderDto?> EditInvoice(int orderId, Dictionary<string, object> changes, string reason, string changedBy)
    {
        // Log all changes to InvoiceAuditLog
        // Recalculate totals
        // Track refund/charge difference
        
        var order = new OrderDto
        {
            Id = orderId,
            Status = "InvoiceEdited",
            TotalAmount = 500.00m,
            CreatedAt = DateTime.Now
        };
        return Task.FromResult<OrderDto?>(order);
    }

    public Task<OrderDto?> UpdateShippingInfo(int orderId, string trackingCode, string shippingCompany)
    {
        // Change status to "Shipped"
        // Generate tracking link
        // Send SMS to user with tracking info
        
        var order = new OrderDto
        {
            Id = orderId,
            Status = "Shipped",
            PaymentTrackingCode = trackingCode,
            TotalAmount = 500.00m,
            CreatedAt = DateTime.Now
        };
        return Task.FromResult<OrderDto?>(order);
    }

    public Task<IEnumerable<OrderDto>> GetUserOrders(int userId)
    {
        var orders = new List<OrderDto>
        {
            new OrderDto
            {
                Id = 1,
                UserId = userId,
                CampaignId = 1,
                TotalAmount = 500.00m,
                Status = "PaymentConfirmed",
                CreatedAt = DateTime.Now.AddDays(-1)
            },
            new OrderDto
            {
                Id = 2,
                UserId = userId,
                CampaignId = 1,
                TotalAmount = 750.00m,
                Status = "Shipped",
                CreatedAt = DateTime.Now.AddDays(-5)
            }
        };
        return Task.FromResult<IEnumerable<OrderDto>>(orders);
    }

    public Task<OrderDto?> GetOrderDetails(int orderId)
    {
        var order = new OrderDto
        {
            Id = orderId,
            UserId = 1,
            CampaignId = 1,
            TotalAmount = 500.00m,
            DiscountAmount = 50.00m,
            TaxAmount = 45.00m,
            ShippingCost = 30.00m,
            ShippingMethod = "Standard",
            PaymentMethod = "Online",
            Status = "PaymentConfirmed",
            CreatedAt = DateTime.Now,
            OrderItems = new List<OrderItemDto>()
        };
        return Task.FromResult<OrderDto?>(order);
    }
}
