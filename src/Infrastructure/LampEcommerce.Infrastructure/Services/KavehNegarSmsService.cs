using Kavenegar;
using LampEcommerce.Application.Models;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Application.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LampEcommerce.Infrastructure.Services;

public class KavehNegarSmsService : ISmsService
{
    private readonly SmsSettings _smsSettings;
    private readonly KavenegarApi _kavenegarApi;
    private readonly ILogger<KavehNegarSmsService> _logger;
    private string GetTemplateName(string key, string defaultValue)
    {
        if (_smsSettings.Templates != null && _smsSettings.Templates.TryGetValue(key, out var templateName))
        {
            return templateName;
        }
        return defaultValue;
    }

    public KavehNegarSmsService(IOptions<SmsSettings> smsSettings, ILogger<KavehNegarSmsService> logger)
    {
        _smsSettings = smsSettings.Value;
        _kavenegarApi = new KavenegarApi(_smsSettings.ApiKey);
        _logger = logger;
    }

    public async Task<bool> SendTemporaryPassword(string mobile, string temporaryPassword)
    {
        try
        {
            var template = GetTemplateName("TemporaryPassword", "passwordtmp");
            var result = await Task.Run(() => _kavenegarApi.VerifyLookup(mobile, temporaryPassword, template));
            return result != null;
        }
        catch (Kavenegar.Exceptions.ApiException ex)
        {
            _logger.LogError(ex, "Error sending temporary password SMS to {Mobile}", mobile);
            return false;
        }
        catch (Kavenegar.Exceptions.HttpException ex)
        {
            _logger.LogError(ex, "Error sending temporary password SMS to {Mobile}", mobile);
            return false;
        }
    }

    public async Task<ApiResponse> SendSmsAsync(SendSmsRequest request)
    {
        try
        {
            if (request.TemplateId.HasValue)
            {
                var templateName = request.TemplateId.Value == 1 
                    ? GetTemplateName("TemporaryPassword", "passwordtmp") 
                    : "unknown";
                
                var result = await Task.Run(() => _kavenegarApi.VerifyLookup(request.PhoneNumber, request.Message, templateName));
                return result == null
                    ? throw new Kavenegar.Exceptions.ApiException("result is null", 0)
                    : new ApiResponse { Success = true, Message = "SMS sent successfully." };
            }
            else
            {
                var result = await Task.Run(() => _kavenegarApi.Send(_smsSettings.SenderId, request.PhoneNumber, request.Message));
                return result == null
                    ? throw new Kavenegar.Exceptions.ApiException("result is null", 0)
                    : new ApiResponse { Success = true, Message = "SMS sent successfully." };
            }
        }
        catch (Kavenegar.Exceptions.ApiException ex)
        {
            _logger.LogError(ex, "Error sending SMS to {Receptor}", request.PhoneNumber);
            return new ApiResponse { Success = false, Message = "Failed to send SMS." };
        }
        catch (Kavenegar.Exceptions.HttpException ex)
        {
            _logger.LogError(ex, "Error sending SMS to {Receptor}", request.PhoneNumber);
            return new ApiResponse { Success = false, Message = "Failed to connect to Kavenegar server." };
        }
    }

    public Task<IEnumerable<SmsTemplateDto>> GetTemplatesAsync()
    {
        var templates = new List<SmsTemplateDto>
        {
            new SmsTemplateDto { Id = 1, Name = "Temporary Password", Template = "کد تایید موقت: {0}", IsActive = true }
        };
        return Task.FromResult<IEnumerable<SmsTemplateDto>>(templates);
    }
}
