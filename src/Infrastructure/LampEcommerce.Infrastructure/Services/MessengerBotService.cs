using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LampEcommerce.Infrastructure.Services;

public class MessengerBotService : IMessengerBotService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<MessengerBotService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string? _baleToken;
    private readonly string? _telegramToken;

    public MessengerBotService(
        IUserRepository userRepository,
        ILogger<MessengerBotService> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("MessengerBotClient");
        
        _baleToken = configuration["BotSettings:BaleToken"];
        _telegramToken = configuration["BotSettings:TelegramToken"];
    }

    public async Task<bool> SendMessageAsync(string channel, string chatId, string message)
    {
        _logger.LogInformation("Sending message via {Channel} to ChatId: {ChatId}. Message: {Message}", channel, chatId, message);

        try
        {
            if (channel.Equals("Bale", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(_baleToken))
                {
                    _logger.LogInformation("[SIMULATED BALE BOT] Token not configured. Simulated send success to {ChatId}: {Message}", chatId, message);
                    return true;
                }

                var url = $"https://tapi.bale.ai/bot{_baleToken}/sendMessage";
                var payload = new { chat_id = chatId, text = message };
                var response = await _httpClient.PostAsync(url, new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
                return response.IsSuccessStatusCode;
            }
            else if (channel.Equals("Telegram", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(_telegramToken))
                {
                    _logger.LogInformation("[SIMULATED TELEGRAM BOT] Token not configured. Simulated send success to {ChatId}: {Message}", chatId, message);
                    return true;
                }

                var url = $"https://api.telegram.org/bot{_telegramToken}/sendMessage";
                var payload = new { chat_id = chatId, text = message };
                var response = await _httpClient.PostAsync(url, new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
                return response.IsSuccessStatusCode;
            }


            _logger.LogWarning("Unknown bot channel: {Channel}", channel);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send message via {Channel} to {ChatId}", channel, chatId);
            return false;
        }
    }

    public async Task<bool> SendOtpFallbackAsync(string phoneNumber, string otp)
    {
        var user = await _userRepository.GetByPhoneNumberAsync(phoneNumber);
        if (user == null)
        {
            _logger.LogWarning("User with phone number {PhoneNumber} not found for OTP fallback.", phoneNumber);
            return false;
        }

        var message = $"کد تایید شما برای ورود به لایت مارکت: {otp}";

        // Try Bale first
        if (!string.IsNullOrEmpty(user.BaleChatId))
        {
            _logger.LogInformation("Attempting OTP fallback via Bale for phone {PhoneNumber}", phoneNumber);
            return await SendMessageAsync("Bale", user.BaleChatId, message);
        }



        // Try Telegram third
        if (!string.IsNullOrEmpty(user.TelegramChatId))
        {
            _logger.LogInformation("Attempting OTP fallback via Telegram for phone {PhoneNumber}", phoneNumber);
            return await SendMessageAsync("Telegram", user.TelegramChatId, message);
        }

        _logger.LogWarning("User with phone number {PhoneNumber} has no registered messenger Chat IDs for OTP fallback.", phoneNumber);
        return false;
    }
}
