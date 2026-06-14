using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Models;

namespace LampEcommerce.Application.Services;

public class OrderService : IOrderService
{
    public Task<OrderDto?> CreateOrderAsync(int userId, CheckoutRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<OrderDto?> ConfirmPaymentAsync(ConfirmPaymentRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<OrderDto?> UpdateOrderStatusAsync(UpdateOrderStatusRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse> EditInvoiceAsync(EditInvoiceRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<OrderDto?> GetOrderByIdAsync(int orderId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId)
    {
        throw new NotImplementedException();
    }
}
