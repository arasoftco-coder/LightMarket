using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace LampEcommerce.Application.Services;

public class MagicLinkService
{
    private readonly string _jwtSecret = "YourSuperSecretKeyForMagicLinkGeneration2024!";
    private readonly int _defaultExpiryMinutes = 30;

    public Task<PaymentLinkResult> GeneratePaymentLinkAsync(int orderId, int userId, int expiryMinutes = 30)
    {
        var expiryTime = expiryMinutes > 0 ? expiryMinutes : _defaultExpiryMinutes;
        
        var token = GenerateJwtToken(orderId, userId, expiryTime);
        var paymentUrl = $"/invoice/checkout?token={token}";
        
        var result = new PaymentLinkResult
        {
            Token = token,
            PaymentUrl = paymentUrl,
            ExpiryTime = DateTime.Now.AddMinutes(expiryTime),
            IsSingleUse = true
        };
        
        return Task.FromResult(result);
    }

    public Task<PaymentLinkValidationResult> ValidatePaymentLinkAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSecret);
            
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = "LightMarket",
                ValidateAudience = true,
                ValidAudience = "LightMarket",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;
            
            // Extract claims
            var orderIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "OrderId");
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId");
            var purposeClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "Purpose");
            
            if (orderIdClaim == null || userIdClaim == null || purposeClaim?.Value != "Payment")
            {
                return Task.FromResult(new PaymentLinkValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Invalid token claims"
                });
            }
            
            // In Phase 3: Mark token as used (single-use) in database/cache
            
            return Task.FromResult(new PaymentLinkValidationResult
            {
                IsValid = true,
                OrderId = int.Parse(orderIdClaim.Value),
                UserId = int.Parse(userIdClaim.Value),
                ExpiryTime = jwtToken.ValidTo
            });
        }
        catch (SecurityTokenExpiredException)
        {
            return Task.FromResult(new PaymentLinkValidationResult
            {
                IsValid = false,
                ErrorMessage = "Token has expired"
            });
        }
        catch (Exception ex)
        {
            return Task.FromResult(new PaymentLinkValidationResult
            {
                IsValid = false,
                ErrorMessage = $"Token validation failed: {ex.Message}"
            });
        }
    }

    private string GenerateJwtToken(int orderId, int userId, int expiryMinutes)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("OrderId", orderId.ToString()),
            new Claim("UserId", userId.ToString()),
            new Claim("Purpose", "Payment"),
            new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.Now.AddMinutes(expiryMinutes).ToUnixTimeSeconds().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: "LightMarket",
            audience: "LightMarket",
            claims: claims,
            expires: DateTime.Now.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class PaymentLinkResult
{
    public string Token { get; set; } = string.Empty;
    public string PaymentUrl { get; set; } = string.Empty;
    public DateTime ExpiryTime { get; set; }
    public bool IsSingleUse { get; set; }
}

public class PaymentLinkValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public DateTime ExpiryTime { get; set; }
}
