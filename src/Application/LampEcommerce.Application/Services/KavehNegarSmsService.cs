using Kavenegar;
using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Application.Models;
using LampEcommerce.WebAPI.Settings;
using Microsoft.Extensions.Options;

namespace LampEcommerce.Application.Services;

public class KavehNegarSmsService : ISmsService
{
    private readonly SmsSettings _smsSettings;
    private readonly KavenegarApi _kavenegarApi;
    private readonly ILogger<KavehNegarSmsService> _logger;

    public KavehNegarSmsService(IOptions<SmsSettings> smsSettings, ILogger<KavehNegarSmsService> logger)
    {
        _smsSettings = smsSettings.Value;
        _kavenegarApi = new KavenegarApi(_smsSettings.ApiKey);
        _logger = logger;
    }

    public async Task<ApiResponse> SendSmsAsync(SendSmsRequest request)
    {
        try
        {
            string message = request.Message;

            // If TemplateId is provided, load template and replace variables
            if (request.TemplateId.HasValue)
            {
                var templateResponse = await GetTemplateByIdAsync(request.TemplateId.Value);
                if (templateResponse != null)
                {
                    message = ReplaceTemplateVariables(templateResponse.Template, request);
                }
            }

            // Send SMS using Kavenegar API
            var result = await _kavenegarApi.Send(_smsSettings.SenderId, new[] { request.PhoneNumber }, message);

            if (result != null && result.Return.Status == 200)
            {
                _logger.LogInformation("SMS sent successfully to {PhoneNumber} via Kavenegar", request.PhoneNumber);
                return new ApiResponse
                {
                    Success = true,
                    Message = "SMS sent successfully"
                };
            }
            else
            {
                _logger.LogWarning("SMS sending failed to {PhoneNumber}. Status: {Status}", 
                    request.PhoneNumber, result?.Return.Status);
                return new ApiResponse
                {
                    Success = false,
                    Message = $"SMS sending failed. Status: {result?.Return.Status}"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS to {PhoneNumber}", request.PhoneNumber);
            return new ApiResponse
            {
                Success = false,
                Message = $"Error sending SMS: {ex.Message}"
            };
        }
    }

    public async Task<IEnumerable<SmsTemplateDto>> GetTemplatesAsync()
    {
        try
        {
            // Fetch templates from database or cache
            // For now, return static templates as placeholder
            var templates = new List<SmsTemplateDto>
            {
                new SmsTemplateDto
                {
                    Id = 1,
                    Name = "Order Confirmation",
                    Template = "Dear {CustomerName}, your order #{InvoiceNumber} has been confirmed.",
                    IsActive = true
                },
                new SmsTemplateDto
                {
                    Id = 2,
                    Name = "Payment Received",
                    Template = "Dear {CustomerName}, we received your payment for order #{InvoiceNumber}.",
                    IsActive = true
                },
                new SmsTemplateDto
                {
                    Id = 3,
                    Name = "Shipping Notification",
                    Template = "Dear {CustomerName}, your order #{InvoiceNumber} has been shipped. Tracking: {TrackingCode}",
                    IsActive = true
                },
                new SmsTemplateDto
                {
                    Id = 4,
                    Name = "OTP Verification",
                    Template = "Your verification code is: {OTP}. Valid for 2 minutes.",
                    IsActive = true
                }
            };

            return templates;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching SMS templates");
            return Enumerable.Empty<SmsTemplateDto>();
        }
    }

    private async Task<SmsTemplateDto?> GetTemplateByIdAsync(int templateId)
    {
        var templates = await GetTemplatesAsync();
        return templates.FirstOrDefault(t => t.Id == templateId);
    }

    private string ReplaceTemplateVariables(string template, SendSmsRequest request)
    {
        // This is a simple implementation. In production, you might want to use
        // a more sophisticated templating engine or pass variables explicitly.
        return template
            .Replace("{CustomerName}", "Customer")
            .Replace("{InvoiceNumber}", "N/A")
            .Replace("{TrackingCode}", "N/A")
            .Replace("{OTP}", "123456"); // In production, generate actual OTP
    }
}
