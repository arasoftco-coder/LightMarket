using Kavenegar;
using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Application.Models;
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
            // Use VerifyLookup for OTP sending with templates
            if (request.TemplateId.HasValue && request.TemplateArgs != null)
            {
                var lookupArgs = new List<string>(request.TemplateArgs.Values);
                var result = await _kavenegarApi.VerifyLookup(
                    request.PhoneNumber, 
                    request.TemplateId.Value.ToString(), 
                    lookupArgs.ToArray()
                );

                if (result != null && result.Return.Status == 200)
                {
                    _logger.LogInformation("OTP sent successfully to {PhoneNumber} via Kavenegar VerifyLookup", request.PhoneNumber);
                    return new ApiResponse
                    {
                        Success = true,
                        Message = "OTP sent successfully"
                    };
                }
                else
                {
                    _logger.LogWarning("OTP sending failed to {PhoneNumber}. Status: {Status}", 
                        request.PhoneNumber, result?.Return.Status);
                    return new ApiResponse
                    {
                        Success = false,
                        Message = $"OTP sending failed. Status: {result?.Return.Status}"
                    };
                }
            }
            else
            {
                // Fallback to regular SMS send if no template
                var result = await _kavenegarApi.Send(_smsSettings.SenderNumber, new[] { request.PhoneNumber }, request.Message);

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
        // Kavenegar doesn't provide a direct API to fetch templates
        // Return empty list as this is handled in the admin panel
        return Enumerable.Empty<SmsTemplateDto>();
    }
}
