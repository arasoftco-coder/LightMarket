namespace LampEcommerce.Domain.Entities;

public class InvoiceAuditLog
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string PreviousInvoiceData { get; set; } = string.Empty;
    public string NewInvoiceData { get; set; } = string.Empty;
    public string ChangedBy { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public string Reason { get; set; } = string.Empty;

    // Navigation properties
    public virtual Order Order { get; set; } = null!;
}
