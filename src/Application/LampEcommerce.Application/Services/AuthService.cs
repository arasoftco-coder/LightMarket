using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Models;

namespace LampEcommerce.Application.Services;

public class AuthService : IAuthService
{
    public Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<OtpResponse> GenerateOtpAsync(GenerateOtpRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<AuthResponse> VerifyOtpAsync(VerifyOtpRequest request)
    {
        throw new NotImplementedException();
    }
}
