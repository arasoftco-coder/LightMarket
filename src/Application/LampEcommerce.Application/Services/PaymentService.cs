using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Interfaces;

namespace LampEcommerce.Application.Services;

public interface IPaymentService
{
    Task<string> CreatePaymentRequest(int orderId);
    Task<bool> VerifyPayment(string referenceId);
}

public class PaymentService : IPaymentService
{
    public Task<string> CreatePaymentRequest(int orderId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> VerifyPayment(string referenceId)
    {
        throw new NotImplementedException();
    }
}
