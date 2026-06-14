using LampEcommerce.Application.Models;

namespace LampEcommerce.Application.Interfaces;

public interface ISmsService
{
    Task<ApiResponse> SendSmsAsync(SendSmsRequest request);
    Task<IEnumerable<SmsTemplateDto>> GetTemplatesAsync();
}

public class SmsTemplateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
