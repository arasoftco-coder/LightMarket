using Kavenegar;
using LampEcommerce.Application.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LampEcommerce.Application.Services;

public class KavehNegarSmsService
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


}
