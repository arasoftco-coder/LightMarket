namespace LampEcommerce.Application.DTOs;

using System.Linq;
using System.Collections.Generic;

public class UserDto
{
    public int Id { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string Role { get; set; } = "Customer";
    public DateTime CreatedAt { get; set; }
}

public class OtpVerifyResult
{
    public UserDto User { get; set; } = new UserDto();
    public bool IsNewUser { get; set; }
}

public class AddressDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverPhone { get; set; } = string.Empty;
}

public class SupplierDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string? ContactInfo { get; set; }
    public bool RequiresTrackingCode { get; set; }
}

public class CampaignDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public int SupplierId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public string? AllowedProvinces { get; set; }
    public string? AllowedCities { get; set; }
    public int MinOrderQty { get; set; }
    public int MaxOrderQty { get; set; }
}

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
}

public class CampaignProductDto
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
    
    public ProductDto? Product { get; set; }
}

public class OrderItemDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    
    public ProductDto? Product { get; set; }
}

public class OrderDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CampaignId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public string ShippingMethod { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string? PaymentTrackingCode { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
    
    public UserDto? User { get; set; }
    public CampaignDto? Campaign { get; set; }
    public ICollection<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
}

public class InvoiceAuditLogDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string PreviousInvoiceData { get; set; } = string.Empty;
    public string NewInvoiceData { get; set; } = string.Empty;
    public string ChangedBy { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class CartItemDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime AddedAt { get; set; }
    
    public ProductDto? Product { get; set; }
}

public class CartDto
{
    public int UserId { get; set; }
    public ICollection<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    public int TotalCount => Items.Sum(i => i.Quantity);
    public decimal TotalAmount => Items.Sum(i => i.UnitPrice * i.Quantity);
}

public class TicketDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = "Open";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public UserDto? User { get; set; }
}

public class SmsTemplateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Template { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CampaignReportDto
{
    public int CampaignId { get; set; }
    public string CampaignName { get; set; } = string.Empty;
    public int TotalOrders { get; set; }
    public int TotalUsers { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalDiscount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
