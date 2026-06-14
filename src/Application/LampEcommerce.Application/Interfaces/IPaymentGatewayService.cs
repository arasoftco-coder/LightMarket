namespace LampEcommerce.Application.Interfaces;

public interface IPaymentGatewayService
{
    Task<string> CreatePaymentRequest(int orderId, decimal amount, string callbackUrl);
    Task<PaymentResult> VerifyPayment(string authority, string status);
}

public class PaymentResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
