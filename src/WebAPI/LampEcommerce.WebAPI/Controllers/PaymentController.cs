using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LampEcommerce.Application.Interfaces;

namespace LampEcommerce.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentGatewayService _paymentGatewayService;
    private readonly IMagicLinkService _magicLinkService;
    private readonly IOrderService _orderService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(
        IPaymentGatewayService paymentGatewayService, 
        IMagicLinkService magicLinkService, 
        IOrderService orderService,
        ILogger<PaymentController> logger)
    {
        _paymentGatewayService = paymentGatewayService;
        _magicLinkService = magicLinkService;
        _orderService = orderService;
        _logger = logger;
    }

    [HttpPost("create-request")]
    [Authorize]
    public async Task<IActionResult> CreatePaymentRequest([FromBody] CreatePaymentRequestDto request)
    {
        try
        {
            var callbackUrl = $"{Request.Scheme}://{Request.Host}/api/payment/verify";
            var paymentUrl = await _paymentGatewayService.CreatePaymentRequest(request.OrderId, request.Amount, callbackUrl);
            return Ok(new { success = true, paymentUrl = paymentUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment request");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentRequest request)
    {
        try
        {
            var result = await _paymentGatewayService.VerifyPayment(request.Authority, request.Status);
            if (result.Success)
            {
                // Extract orderId from authority (AUTH_{orderId}_{guid})
                var parts = request.Authority.Split('_');
                if (parts.Length >= 2 && int.TryParse(parts[1], out var orderId))
                {
                    // Extract tracking code from result message or generate a default one
                    var trackingCode = result.Message.Contains("Tracking Code: ")
                        ? result.Message.Substring(result.Message.IndexOf("Tracking Code: ") + 15)
                        : $"TRK_{Guid.NewGuid():N}";

                    await _orderService.ConfirmPayment(orderId, trackingCode);
                }
            }
            return Ok(new { success = result.Success, message = result.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying payment");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("generate-magic-link")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GenerateMagicLink([FromBody] GenerateMagicLinkRequest request)
    {
        try
        {
            var link = await _magicLinkService.GeneratePaymentLink(request.OrderId, request.UserId, request.ExpiryMinutes);
            return Ok(new { success = true, magicLink = link });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating magic link");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}

public class CreatePaymentRequestDto { public int OrderId { get; set; } public decimal Amount { get; set; } }
public class VerifyPaymentRequest { public string Authority { get; set; } = string.Empty; public string Status { get; set; } = string.Empty; }
public class GenerateMagicLinkRequest { public int OrderId { get; set; } public int UserId { get; set; } public int ExpiryMinutes { get; set; } = 30; }
