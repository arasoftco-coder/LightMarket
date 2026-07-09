using System;

namespace LampEcommerce.Domain.Entities;

public class PaymentMethod
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "CardToCard"; // Gateway | CardToCard
    public string GatewayName { get; set; } = "CardToCard"; // ZarinPal, Pasargad, Mellat, Saman, CardToCard
    public string GatewayConfig { get; set; } = "{}"; // JSON string storing dynamic configuration (card details or gateway merchant/terminal details)
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
