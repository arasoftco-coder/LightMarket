namespace LampEcommerce.Domain.Entities;

public class Campaign
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public string? AllowedProvinces { get; set; }  // Comma-separated list
    public string? AllowedCities { get; set; }      // Comma-separated list
    public int MinOrderQty { get; set; }
    public int MaxOrderQty { get; set; }

    // Navigation properties
    public virtual Supplier Supplier { get; set; } = null!;
    public virtual ICollection<CampaignProduct> CampaignProducts { get; set; } = new List<CampaignProduct>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
