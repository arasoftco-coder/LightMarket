namespace LampEcommerce.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }

    // Navigation properties
    public virtual ICollection<CampaignProduct> CampaignProducts { get; set; } = new List<CampaignProduct>();
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
