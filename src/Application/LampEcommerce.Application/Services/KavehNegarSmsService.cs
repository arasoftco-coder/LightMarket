using Kavenegar;
using LampEcommerce.Application.Models;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Application.DTOs;
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

    public async Task<ApiResponse> SendTemporaryPassword(string receptor, string password)
    {
        try
        {
            var result = _kavenegarApi.VerifyLookup(receptor, password, KaveNegarTemplate.TemporaryPassword);
            return result == null
                ? throw new Kavenegar.Exceptions.ApiException("result is null", 0)
                : new ApiResponse { Success = true };
        }
        catch (Kavenegar.Exceptions.ApiException ex) //return not 200
        {
            _logger.LogError(ex, "Error sending temporary password SMS to {Receptor}", receptor);
            return new ApiResponse { Success = false, Message = "Failed to send temporary password SMS." };
        }
        catch (Kavenegar.Exceptions.HttpException ex) //something went wrong during the HTTP request
        {
            _logger.LogError(ex, "Error sending temporary password SMS to {Receptor}", receptor);
            return new ApiResponse { Success = false, Message = "Failed to connect to Kavenegar server." };
        }

    }

    public async Task<ApiResponse> SendSmsAsync(SendSmsRequest request)
    {
        try
        {
            if (request.TemplateId.HasValue)
            {
                // Get template name from TemplateId
                var templateName = request.TemplateId.Value == 1 
                    ? KaveNegarTemplate.TemporaryPassword 
                    : "unknown";
                
                var result = _kavenegarApi.VerifyLookup(request.PhoneNumber, request.Message, templateName);
                return result == null
                    ? throw new Kavenegar.Exceptions.ApiException("result is null", 0)
                    : new ApiResponse { Success = true, Message = "SMS sent successfully." };
            }
            else
            {
                var result = _kavenegarApi.Send(_smsSettings.SenderId, request.PhoneNumber, request.Message);
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
        // Kavenegar does not provide an API to list templates, returning empty list
        var templates = new List<SmsTemplateDto>
        {
            new SmsTemplateDto { Id = 1, Name = "Temporary Password", Template = "کد تایید موقت: {0}", IsActive = true }
        };
        return Task.FromResult<IEnumerable<SmsTemplateDto>>(templates);
    }


}
