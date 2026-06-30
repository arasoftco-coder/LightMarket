using System.Threading.Tasks;

namespace LampEcommerce.Application.Interfaces;

public interface IMessengerBotService
{
    Task<bool> SendMessageAsync(string channel, string chatId, string message);
    Task<bool> SendOtpFallbackAsync(string phoneNumber, string otp);
}
