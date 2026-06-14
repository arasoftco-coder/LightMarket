namespace LampEcommerce.Domain.Entities;

public class CampaignProduct
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public int ProductId { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal Discount { get; set; }
    public int Stock { get; set; }
    public int MinQtyPerUser { get; set; }
    public int MaxQtyPerUser { get; set; }

    // Navigation properties
    public virtual Campaign Campaign { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}
