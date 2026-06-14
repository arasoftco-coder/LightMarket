namespace LampEcommerce.Application.Interfaces;

public interface IMagicLinkService
{
    Task<string> GeneratePaymentLink(int orderId, int userId, int expiryMinutes = 30);
    Task<MagicLinkResult?> ValidatePaymentLink(string token);
}

public class MagicLinkResult
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public bool IsValid { get; set; }
}
