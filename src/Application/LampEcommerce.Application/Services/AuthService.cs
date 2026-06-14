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

    public AuthService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // In Phase 3, this will be implemented with database operations
        var response = new AuthResponse
        {
            Success = true,
            Message = "Registration successful. Please verify OTP.",
            User = new UserDto
            {
                PhoneNumber = request.PhoneNumber,
                FullName = request.FullName
            }
        };
        return Task.FromResult(response);
    }

    public Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // In Phase 3, this will validate credentials from database
        var token = GenerateJwtToken(1, request.PhoneNumber);
        
        var response = new AuthResponse
        {
            Success = true,
            Token = token,
            Message = "Login successful",
            User = new UserDto
            {
                PhoneNumber = request.PhoneNumber
            }
        };
        return Task.FromResult(response);
    }

    public Task<OtpResponse> GenerateOtpAsync(GenerateOtpRequest request)
    {
        var random = new Random();
        var otp = random.Next(10000, 99999).ToString();
        var cacheKey = $"OTP_{request.PhoneNumber}";
        
        _cache.Set(cacheKey, otp, OtpExpiryTime);
        
        var response = new OtpResponse
        {
            Success = true,
            Message = "OTP sent successfully",
            OtpId = cacheKey
        };
        return Task.FromResult(response);
    }

    public Task<AuthResponse> VerifyOtpAsync(VerifyOtpRequest request)
    {
        var cacheKey = $"OTP_{request.PhoneNumber}";
        
        if (!_cache.TryGetValue(cacheKey, out string? storedOtp))
        {
            return Task.FromResult(new AuthResponse
            {
                Success = false,
                Message = "OTP expired or not found"
            });
        }
        
        if (storedOtp != request.Otp)
        {
            return Task.FromResult(new AuthResponse
            {
                Success = false,
                Message = "Invalid OTP"
            });
        }
        
        _cache.Remove(cacheKey);
        var token = GenerateJwtToken(1, request.PhoneNumber);
        
        var response = new AuthResponse
        {
            Success = true,
            Token = token,
            Message = "OTP verified successfully",
            User = new UserDto
            {
                PhoneNumber = request.PhoneNumber
            }
        };
        return Task.FromResult(response);
    }

    public Task<bool> SetPasswordAsync(int userId, string password)
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
