using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Models;

namespace LampEcommerce.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<OtpResponse> GenerateOtpAsync(GenerateOtpRequest request);
    Task<AuthResponse> VerifyOtpAsync(VerifyOtpRequest request);
}
