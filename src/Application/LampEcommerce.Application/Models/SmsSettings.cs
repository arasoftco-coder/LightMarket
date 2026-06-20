namespace LampEcommerce.Application.Models;

public class SmsSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
}
