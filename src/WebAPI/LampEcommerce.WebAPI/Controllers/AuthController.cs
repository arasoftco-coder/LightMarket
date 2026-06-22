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
    private readonly ISmsService _smsService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, IJwtTokenGenerator jwtTokenGenerator, ISmsService smsService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _smsService = smsService;
        _logger = logger;
    }

    [HttpPost("send-temporary-password")]
    public async Task<IActionResult> SendTemporaryPassword([FromBody] SendTemporaryPasswordRequest request)
    {
        try
        {
            var result = await _smsService.SendTemporaryPassword(request.PhoneNumber, request.TemporaryPassword);
            if (result)
                return Ok(new { success = true, message = "Temporary password sent successfully" });
            else
                return BadRequest(new { success = false, message = "Failed to send temporary password" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending temporary password");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                return BadRequest(new { success = false, message = "Phone number is required" });
            }

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
            var result = await _authService.VerifyOTP(request.PhoneNumber, request.Code);
            if (result == null)
                return Unauthorized(new { success = false, message = "Invalid OTP" });

            var user = result.User;
            var token = _jwtTokenGenerator.GenerateToken(user.Id, user.PhoneNumber);
            return Ok(new { success = true, token = token, isNewUser = result.IsNewUser, user = new { user.Id, user.PhoneNumber, user.FullName } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying OTP");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("login-password")]
    public async Task<IActionResult> LoginWithPassword([FromBody] PasswordLoginRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.PhoneNumber) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { success = false, message = "Phone number and password are required" });

            var user = await _authService.LoginWithPassword(request.PhoneNumber, request.Password);
            if (user == null)
                return Unauthorized(new { success = false, message = "Invalid phone number or password" });

            var token = _jwtTokenGenerator.GenerateToken(user.Id, user.PhoneNumber);
            return Ok(new { success = true, token = token, user = new { user.Id, user.PhoneNumber, user.FullName } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging in with password");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var user = await _authService.Register(request.PhoneNumber, request.FullName);
            var token = _jwtTokenGenerator.GenerateToken(user.Id, user.PhoneNumber);
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
public class SendTemporaryPasswordRequest { public string PhoneNumber { get; set; } = string.Empty; public string TemporaryPassword { get; set; } = string.Empty; }
public class PasswordLoginRequest { public string PhoneNumber { get; set; } = string.Empty; public string Password { get; set; } = string.Empty; }
