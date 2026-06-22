using System.Security.Cryptography;
using System.Text;
using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace LampEcommerce.Application.Services;

public class AuthService : IAuthService
{
    private readonly IMemoryCache _cache;
    private readonly TimeSpan OtpExpiryTime = TimeSpan.FromMinutes(2);
    private readonly ISmsService _smsService;
    private readonly IUserRepository _userRepository;

    public AuthService(IMemoryCache cache, ISmsService smsService, IUserRepository userRepository)
    {
        _cache = cache;
        _smsService = smsService;
        _userRepository = userRepository;
    }

    public async Task<UserDto?> GenerateOTP(string phoneNumber)
    {
        // Generate 5-digit OTP
        var otp = new Random().Next(10000, 99999).ToString();

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

    public async Task<OtpVerifyResult?> VerifyOTP(string phoneNumber, string code)
    {
        // Verify OTP from cache
        if (!_cache.TryGetValue(phoneNumber, out var storedOtp) || storedOtp?.ToString() != code)
        {
            return null;
        }

        // OTP is valid, remove it from cache
        _cache.Remove(phoneNumber);

        // Register the user automatically if they don't exist yet
        var user = await _userRepository.GetByPhoneNumberAsync(phoneNumber);
        var isNewUser = false;
        if (user == null)
        {
            user = new User
            {
                PhoneNumber = phoneNumber,
                FullName = string.Empty,
                Role = "User",
                PasswordHash = string.Empty,
                PasswordSalt = string.Empty,
                CreatedAt = DateTime.UtcNow
            };
            user = await _userRepository.AddAsync(user);
            isNewUser = true;
        }

        return new OtpVerifyResult
        {
            User = MapToDto(user),
            IsNewUser = isNewUser
        };
    }

    public async Task<UserDto> Register(string phoneNumber, string fullName)
    {
        var user = await _userRepository.GetByPhoneNumberAsync(phoneNumber);
        if (user == null)
        {
            user = new User
            {
                PhoneNumber = phoneNumber,
                FullName = fullName,
                Role = "User",
                PasswordHash = string.Empty,
                PasswordSalt = string.Empty,
                CreatedAt = DateTime.UtcNow
            };
            user = await _userRepository.AddAsync(user);
        }
        else
        {
            user.FullName = fullName;
            await _userRepository.UpdateAsync(user);
        }

        return MapToDto(user);
    }

    public async Task<bool> SetPassword(int userId, string password)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        var salt = GenerateSalt();
        user.PasswordHash = HashPassword(password, salt);
        user.PasswordSalt = Convert.ToBase64String(salt);
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<UserDto?> LoginWithPassword(string phoneNumber, string password)
    {
        var user = await _userRepository.GetByPhoneNumberAsync(phoneNumber);
        if (user == null || string.IsNullOrEmpty(user.PasswordHash) || string.IsNullOrEmpty(user.PasswordSalt))
        {
            return null;
        }

        var salt = Convert.FromBase64String(user.PasswordSalt);
        var hashedPassword = HashPassword(password, salt);
        if (!CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(hashedPassword),
                Encoding.UTF8.GetBytes(user.PasswordHash)))
        {
            return null;
        }

        return MapToDto(user);
    }

    private static UserDto MapToDto(User user) => new UserDto
    {
        Id = user.Id,
        PhoneNumber = user.PhoneNumber,
        FullName = user.FullName,
        Avatar = user.Avatar,
        Role = user.Role,
        CreatedAt = user.CreatedAt
    };

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
