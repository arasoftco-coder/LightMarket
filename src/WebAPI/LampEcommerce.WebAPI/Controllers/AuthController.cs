using Kavenegar;
using Microsoft.AspNetCore.Mvc;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Application.DTOs;
using LampEcommerce.WebAPI.Services;

namespace LampEcommerce.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, IJwtTokenGenerator jwtTokenGenerator, ILogger<AuthController> logger)
    {
        _authService = authService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _logger = logger;
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
    {
        try
        {
            var result = await _authService.GenerateOTP(request.PhoneNumber);
            return Ok(new { success = true, message = "OTP sent successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending OTP");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        try
        {
            var user = await _authService.VerifyOTP(request.PhoneNumber, request.Code);
            if (user == null)
                return Unauthorized(new { success = false, message = "Invalid OTP" });

            var token = _jwtTokenGenerator.GenerateToken(user.Id, user.PhoneNumber, user.Role);
            return Ok(new { success = true, token = token, user = new { user.Id, user.PhoneNumber, user.FullName } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying OTP");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var user = await _authService.Register(request.PhoneNumber, request.FullName);
            var token = _jwtTokenGenerator.GenerateToken(user.Id, user.PhoneNumber, user.Role);
            return Ok(new { success = true, token = token, user = new { user.Id, user.PhoneNumber, user.FullName } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("set-password")]
    public async Task<IActionResult> SetPassword([FromBody] SetPasswordRequest request)
    {
        try
        {
            await _authService.SetPassword(request.UserId, request.Password);
            return Ok(new { success = true, message = "Password set successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting password");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}

public class SendOtpRequest { public string PhoneNumber { get; set; } = string.Empty; }
public class VerifyOtpRequest { public string PhoneNumber { get; set; } = string.Empty; public string Code { get; set; } = string.Empty; }
public class RegisterRequest { public string PhoneNumber { get; set; } = string.Empty; public string FullName { get; set; } = string.Empty; }
public class SetPasswordRequest { public int UserId { get; set; } public string Password { get; set; } = string.Empty; }
