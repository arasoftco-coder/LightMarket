using LampEcommerce.Application.DTOs;

namespace LampEcommerce.Application.Interfaces;

public interface IAuthService
{
    Task<UserDto?> GenerateOTP(string phoneNumber);
    Task<UserDto?> VerifyOTP(string phoneNumber, string code);
    Task<UserDto> Register(string phoneNumber, string fullName);
    Task<bool> SetPassword(int userId, string password);
}
