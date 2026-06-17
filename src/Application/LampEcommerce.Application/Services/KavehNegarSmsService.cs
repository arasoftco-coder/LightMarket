using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Application.Models;

namespace LampEcommerce.Application.Services
{
    internal class KavehNegarSmsService:ISmsService
    {
        public Task<ApiResponse> SendSmsAsync(SendSmsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SmsTemplateDto>> GetTemplatesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
