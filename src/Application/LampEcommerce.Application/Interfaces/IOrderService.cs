using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Models;

namespace LampEcommerce.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto?> CreateOrderAsync(int userId, CheckoutRequest request);
    Task<OrderDto?> ConfirmPaymentAsync(ConfirmPaymentRequest request);
    Task<OrderDto?> UpdateOrderStatusAsync(UpdateOrderStatusRequest request);
    Task<ApiResponse> EditInvoiceAsync(EditInvoiceRequest request);
    Task<OrderDto?> GetOrderByIdAsync(int orderId);
    Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId);
    Task<OrderDto?> UpdateShippingInfoAsync(int orderId, string trackingCode, string shippingCompany);
}
