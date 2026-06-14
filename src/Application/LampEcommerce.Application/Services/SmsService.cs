using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Application.Models;

namespace LampEcommerce.Application.Services;

public class SmsService : ISmsService
{
    public Task<ApiResponse> SendSmsAsync(SendSmsRequest request)
    {
        // Load template from database if TemplateId is provided
        // Replace variables ({CustomerName}, {InvoiceNumber}, etc.)
        // Call SMS provider API (placeholder for now)
        // Log SMS sent
        
        var message = request.Message;
        if (request.TemplateId.HasValue)
        {
            // In Phase 3, load template and replace variables
            message = $"[Template {request.TemplateId}] {request.Message}";
        }
        
        // Placeholder for SMS provider API call
        Console.WriteLine($"Sending SMS to {request.PhoneNumber}: {message}");
        
        var response = new ApiResponse
        {
            Success = true,
            Message = $"SMS sent successfully to {request.PhoneNumber}"
        };
        return Task.FromResult(response);
    }

    public Task<IEnumerable<SmsTemplateDto>> GetTemplatesAsync()
    {
        // Return all active templates
        var templates = new List<SmsTemplateDto>
        {
            new SmsTemplateDto
            {
                Id = 1,
                Name = "Order Confirmation",
                Content = "Dear {CustomerName}, your order #{InvoiceNumber} has been confirmed."
            },
            new SmsTemplateDto
            {
                Id = 2,
                Name = "Payment Received",
                Content = "Dear {CustomerName}, we received your payment for order #{InvoiceNumber}."
            },
            new SmsTemplateDto
            {
                Id = 3,
                Name = "Shipping Notification",
                Content = "Dear {CustomerName}, your order #{InvoiceNumber} has been shipped. Tracking: {TrackingCode}"
            },
            new SmsTemplateDto
            {
                Id = 4,
                Name = "OTP Verification",
                Content = "Your verification code is: {OTP}. Valid for 2 minutes."
            }
        };
        return Task.FromResult<IEnumerable<SmsTemplateDto>>(templates);
    }
}
