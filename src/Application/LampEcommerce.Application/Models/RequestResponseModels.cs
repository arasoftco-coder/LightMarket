namespace LampEcommerce.Application.Models;

using LampEcommerce.Application.DTOs;

// SMS Settings
public class SmsSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string SenderNumber { get; set; } = string.Empty;
    public int? OtpTemplateId { get; set; }
}

// Auth Requests
public class RegisterRequest
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class VerifyOtpRequest
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
}

public class GenerateOtpRequest
{
    public string PhoneNumber { get; set; } = string.Empty;
}

// Cart Requests
public class AddToCartRequest
{
    public int CampaignProductId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateCartQuantityRequest
{
    public int CartItemId { get; set; }
    public int Quantity { get; set; }
}

public class RemoveFromCartRequest
{
    public int CartItemId { get; set; }
}

// Order Requests
public class CheckoutRequest
{
    public int AddressId { get; set; }
    public string ShippingMethod { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
}

public class ConfirmPaymentRequest
{
    public int OrderId { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
}

public class UpdateOrderStatusRequest
{
    public int OrderId { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class EditInvoiceRequest
{
    public int OrderId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string NewInvoiceData { get; set; } = string.Empty;
}

// SMS Requests
public class SendSmsRequest
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int? TemplateId { get; set; }
    public Dictionary<string, string>? TemplateArgs { get; set; }
}

// Scraper Requests
public class ScrapeProductsRequest
{
    public string SupplierUrl { get; set; } = string.Empty;
    public int SupplierId { get; set; }
}

public class UpdatePricesRequest
{
    public int CampaignProductId { get; set; }
    public decimal NewPurchasePrice { get; set; }
    public decimal NewSellingPrice { get; set; }
}

// Auth Responses
public class AuthResponse
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? Message { get; set; }
    public UserDto? User { get; set; }
}

public class OtpResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? OtpId { get; set; }
}

// Generic Response
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}
