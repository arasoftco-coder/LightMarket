namespace LampEcommerce.Domain.Entities;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string? ContactInfo { get; set; }
    public bool RequiresTrackingCode { get; set; }

    // Navigation properties
    public virtual ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();
}
