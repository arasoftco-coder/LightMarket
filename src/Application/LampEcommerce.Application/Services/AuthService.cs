using System.Security.Cryptography;
using System.Text;
using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Application.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LampEcommerce.Application.Services;

public class AuthService : IAuthService
{
    private readonly IMemoryCache _cache;
    private readonly TimeSpan OtpExpiryTime = TimeSpan.FromMinutes(2);
    private readonly string _jwtSecret = "YourSuperSecretKeyForJWTTokenGeneration2024!";
    private readonly int _tokenExpiryMinutes = 60;
    private readonly ISmsService _smsService;

    public AuthService(IMemoryCache cache, ISmsService smsService)
    {
        _cache = cache;
        _smsService = smsService;
    }

    public async Task<UserDto?> GenerateOTP(string phoneNumber)
    {
        // Generate 4-digit OTP
        var otp = new Random().Next(1000, 9999).ToString();
        
        // Store OTP in cache with expiration
        _cache.Set(phoneNumber, otp, OtpExpiryTime);
        
        // Send OTP via SMS
        await _smsService.SendTemporaryPassword(phoneNumber, otp);
        
        var user = new UserDto
        {
            PhoneNumber = phoneNumber
        };
        return user;
    }

    public Task<UserDto?> VerifyOTP(string phoneNumber, string code)
    {
        // Verify OTP from cache
        if (_cache.TryGetValue(phoneNumber, out var storedOtp))
        {
            if (storedOtp?.ToString() == code)
            {
                // OTP is valid, remove it from cache
                _cache.Remove(phoneNumber);
                var user = new UserDto
                {
                    PhoneNumber = phoneNumber
                };
                return Task.FromResult<UserDto?>(user);
            }
        }
        return Task.FromResult<UserDto?>(null);
    }

    public Task<UserDto> Register(string phoneNumber, string fullName)
    {
        // In Phase 3, this will be implemented with database operations
        var user = new UserDto
        {
            PhoneNumber = phoneNumber,
            FullName = fullName
        };
        return Task.FromResult(user);
    }

    public Task<bool> SetPassword(int userId, string password)
    {
        var salt = GenerateSalt();
        var hashedPassword = HashPassword(password, salt);
        // In Phase 3, store hashedPassword and salt in database for userId
        return Task.FromResult(true);
    }

    private string GenerateJwtToken(int userId, string phoneNumber)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.MobilePhone, phoneNumber),
            new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.Now.AddMinutes(_tokenExpiryMinutes).ToUnixTimeSeconds().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: "LightMarket",
            audience: "LightMarket",
            claims: claims,
            expires: DateTime.Now.AddMinutes(_tokenExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static byte[] GenerateSalt()
    {
        var salt = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        return salt;
    }

    private static string HashPassword(string password, byte[] salt)
    {
        using var sha256 = SHA256.Create();
        var saltedPassword = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();
        var hashedBytes = sha256.ComputeHash(saltedPassword);
        return Convert.ToBase64String(hashedBytes);
    }
}
