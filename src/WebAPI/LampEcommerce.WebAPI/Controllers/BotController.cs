using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using LampEcommerce.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LampEcommerce.WebAPI.Controllers;

[ApiController]
[Route("api/bot")]
public class BotController(
    IUserRepository userRepository,
    IMessengerBotService messengerBotService,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<BotController> logger) : ControllerBase
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMessengerBotService _messengerBotService = messengerBotService;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<BotController> _logger = logger;


    [HttpPost("telegram-webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> TelegramWebhook([FromBody] JsonElement update)
    {
        try
        {
            await ProcessWebhook("Telegram", update);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Telegram webhook");
            return Ok(); // Return 200 to Telegram to prevent retry loops
        }
    }

    [HttpPost("bale-webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> BaleWebhook([FromBody] JsonElement update)
    {
        try
        {
            await ProcessWebhook("Bale", update);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Bale webhook");
            return Ok(); // Return 200 to Bale
        }
    }

    private async Task ProcessWebhook(string channel, JsonElement update)
    {
        if (!update.TryGetProperty("message", out var messageElement)) return;
        if (!messageElement.TryGetProperty("chat", out var chatElement) || !chatElement.TryGetProperty("id", out var chatIdElement)) return;

        string chatId = chatIdElement.ToString();

        // 1. Check if sharing contact
        if (messageElement.TryGetProperty("contact", out var contactElement) && contactElement.TryGetProperty("phone_number", out var phoneElement))
        {
            string rawPhone = phoneElement.GetString() ?? string.Empty;
            string normalizedPhone = NormalizePhoneNumber(rawPhone);

            var user = await _userRepository.GetByPhoneNumberAsync(normalizedPhone);
            if (user != null)
            {
                if (channel == "Telegram")
                {
                    user.TelegramChatId = chatId;
                }
                else
                {
                    user.BaleChatId = chatId;
                }
                await _userRepository.UpdateAsync(user);

                string confirmMsg = $"حساب کاربری شما با شماره {normalizedPhone} با موفقیت به ربات متصل شد! از این پس کدهای ورود و اعلانات فاکتورها از این طریق برای شما ارسال خواهد شد.";
                await _messengerBotService.SendMessageAsync(channel, chatId, confirmMsg);
            }
            else
            {
                string notFoundMsg = $"شماره موبایل {normalizedPhone} در سیستم ثبت نشده است. لطفاً ابتدا در وب‌سایت ثبت‌نام کنید.";
                await _messengerBotService.SendMessageAsync(channel, chatId, notFoundMsg);
            }
            return;
        }

        // 2. Check text commands (like /start)
        if (messageElement.TryGetProperty("text", out var textElement))
        {
            string text = textElement.GetString()?.Trim() ?? string.Empty;
            if (text.StartsWith("/start"))
            {
                string welcomeMsg = "به ربات پشتیبان فروشگاه لایت مارکت خوش آمدید!\n\nبرای اتصال حساب کاربری و دریافت کدهای یک‌بار مصرف ورود و اعلانات فاکتورها، لطفاً با کلیک بر روی دکمه زیر شماره موبایل خود را به اشتراک بگذارید:";
                await SendShareContactKeyboard(channel, chatId, welcomeMsg);
            }
            else
            {
                string reply = "پیام شما دریافت شد. برای اتصال حساب کاربری لطفا دستور /start را ارسال کنید.";
                await _messengerBotService.SendMessageAsync(channel, chatId, reply);
            }
        }
    }

    private static string NormalizePhoneNumber(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return string.Empty;
        phone = phone.Trim().Replace(" ", "").Replace("-", "");
        if (phone.StartsWith("+98")) phone = "0" + phone[3..];
        if (phone.StartsWith("98")) phone = "0" + phone[2..];
        return phone;
    }

    private async Task SendShareContactKeyboard(string channel, string chatId, string text)
    {
        var keyboard = new
        {
            keyboard = new[]
            {
                new[]
                {
                    new
                    {
                        text = "اشتراک‌گذاری شماره موبایل 📱",
                        request_contact = true
                    }
                }
            },
            resize_keyboard = true,
            one_time_keyboard = true
        };

        string? token = channel == "Telegram"
            ? _configuration["BotSettings:TelegramToken"]
            : _configuration["BotSettings:BaleToken"];

        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogInformation("[SIMULATED BOT KEYBOARD] Token not configured. Simulated keyboard sent to {ChatId}: {Text}", chatId, text);
            // Send simulated response text
            await _messengerBotService.SendMessageAsync(channel, chatId, text);
            return;
        }

        string baseUrl = channel == "Telegram" ? "https://api.telegram.org" : "https://tapi.bale.ai";
        string url = $"{baseUrl}/bot{token}/sendMessage";

        var payload = new
        {
            chat_id = chatId,
            text,
            reply_markup = keyboard
        };

        try
        {
            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            await client.PostAsync(url, content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending share contact keyboard via {Channel} to {ChatId}", channel, chatId);
        }
    }
}
