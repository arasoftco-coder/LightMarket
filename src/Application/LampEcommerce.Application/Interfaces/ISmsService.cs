using LampEcommerce.Application.Models;
using LampEcommerce.Application.DTOs;

namespace LampEcommerce.Application.Interfaces;

public interface ISmsService
{
    Task<ApiResponse> SendSmsAsync(SendSmsRequest request);
    Task<IEnumerable<SmsTemplateDto>> GetTemplatesAsync();
    Task<bool> SendTemporaryPassword(string mobile, string temporaryPassword);
}
