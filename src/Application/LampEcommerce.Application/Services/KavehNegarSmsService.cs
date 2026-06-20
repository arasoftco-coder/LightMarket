using Kavenegar;
using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Application.Models;
using Microsoft.Extensions.Logging;
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
            // VerifyLookup requires TemplateId and TemplateArgs
            if (!request.TemplateId.HasValue || request.TemplateArgs == null || !request.TemplateArgs.Any())
            {
                _logger.LogWarning("SMS sending failed: TemplateId and TemplateArgs are required for Kavenegar VerifyLookup");
                return new ApiResponse
                {
                    Success = false,
                    Message = "TemplateId and TemplateArgs are required for Kavenegar VerifyLookup"
                };
            }

            // Convert TemplateArgs dictionary values to string array for VerifyLookup
            var lookupArgs = request.TemplateArgs.Values.ToList();

            // Call Kavenegar VerifyLookup API
            // This method automatically uses the first available sender number from your panel
            var result = await _kavenegarApi.VerifyLookup(
                request.PhoneNumber,
                request.TemplateId.Value.ToString(),
                lookupArgs.ToArray()
            );

            if (result != null && result.Return.Status == 200)
            {
                _logger.LogInformation("SMS sent successfully to {PhoneNumber} via Kavenegar VerifyLookup with template {TemplateId}",
                    request.PhoneNumber, request.TemplateId);
                return new ApiResponse
                {
                    Success = true,
                    Message = "SMS sent successfully"
                };
            }
            else
            {
                _logger.LogWarning("SMS sending failed to {PhoneNumber}. Status: {Status}, Message: {Message}",
                    request.PhoneNumber, result?.Return.Status, result?.Return.Message);
                return new ApiResponse
                {
                    Success = false,
                    Message = $"SMS sending failed. Status: {result?.Return.Status}, Message: {result?.Return.Message}"
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
        // Templates are managed in Kavenegar panel, not via API
        // Return empty list as templates cannot be fetched programmatically
        return Enumerable.Empty<SmsTemplateDto>();
    }
}
