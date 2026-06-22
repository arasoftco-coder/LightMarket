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
        try
        {
            // Register the user automatically if they don't exist yet
            var user = await _userRepository.GetByPhoneNumberAsync(phoneNumber);
            var isNewUser = false;
            if (user == null)
            {
                user = new User
                {
                    PhoneNumber = phoneNumber,
                    FullName = string.Empty,
                    PasswordHash = string.Empty,
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
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        throw new Exception("Uknown error");
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
                PasswordHash = string.Empty,
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

        user.PasswordHash = HashPassword(password);
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<UserDto?> LoginWithPassword(string phoneNumber, string password)
    {
        var user = await _userRepository.GetByPhoneNumberAsync(phoneNumber);
        if (user == null || string.IsNullOrEmpty(user.PasswordHash))
        {
            return null;
        }

        if (!VerifyPassword(password, user.PasswordHash))
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
        CreatedAt = user.CreatedAt
    };

    private static string HashPassword(string password)
    {
        var salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
        {
            var hash = pbkdf2.GetBytes(20);
            var hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            return Convert.ToBase64String(hashBytes);
        }
    }

    private static bool VerifyPassword(string password, string hash)
    {
        var hashBytes = Convert.FromBase64String(hash);
        var salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
        {
            var hash2 = pbkdf2.GetBytes(20);
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash2[i])
                    return false;
            }
            return true;
        }
    }
}
