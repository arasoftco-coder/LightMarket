using LampEcommerce.Application.DTOs;

namespace LampEcommerce.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto?> CreateOrder(int userId, int cartId, int addressId, string shippingMethod, string paymentMethod);
    Task<OrderDto?> ConfirmPayment(int orderId, string trackingCode);
    Task<OrderDto?> EditInvoice(int orderId, Dictionary<string, object> changes, string reason, string changedBy);
    Task<OrderDto?> UpdateShippingInfo(int orderId, string trackingCode, string shippingCompany);
    Task<IEnumerable<OrderDto>> GetUserOrders(int userId);
    Task<OrderDto?> GetOrderDetails(int orderId);
}
