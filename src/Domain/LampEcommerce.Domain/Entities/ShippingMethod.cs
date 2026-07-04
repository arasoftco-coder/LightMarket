namespace LampEcommerce.Domain.Entities;

public class ShippingMethod
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string? ApiKey { get; set; }
    public string? ApiUrl { get; set; }
    public decimal BaseCost { get; set; }
    public bool IsActive { get; set; } = true;
}
