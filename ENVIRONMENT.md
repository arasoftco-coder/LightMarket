# Environment Variables Configuration

This document lists all environment variables required for the LightMarket application.

## Backend (WebAPI)

### Database
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=LightMarket;User Id=sa;Password=YourPassword;TrustServerCertificate=true;"
}
```

### JWT Settings
```json
"JwtSettings": {
  "SecretKey": "your-super-secret-key-at-least-32-characters-long",
  "Issuer": "LightMarket",
  "Audience": "LightMarketUsers",
  "ExpiryMinutes": 1440
}
```

### SMS Settings
```json
"SmsSettings": {
  "ProviderApiKey": "your-sms-provider-api-key",
  "SenderNumber": "10001000",
  "Enabled": true
}
```

### Payment Gateway Settings
```json
"PaymentGatewaySettings": {
  "MerchantId": "your-merchant-id",
  "ApiKey": "your-payment-gateway-api-key",
  "CallbackUrl": "https://yourdomain.com/api/payment/verify",
  "SandboxMode": true
}
```

### Scraper Settings
```json
"ScraperSettings": {
  "MaxRetries": 3,
  "TimeoutSeconds": 30,
  "UserAgent": "LightMarketScraper/1.0"
}
```

## Frontend (Angular)

### environment.ts (Development)
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7001/api',
  otpExpiryMinutes: 2,
  magicLinkExpiryMinutes: 30
};
```

### environment.prod.ts (Production)
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://api.yourdomain.com/api',
  otpExpiryMinutes: 2,
  magicLinkExpiryMinutes: 30
};
```

## Docker Environment Variables

For Docker deployment, use these environment variables:

```bash
# Database
DB_SERVER=localhost
DB_NAME=LightMarket
DB_USER=sa
DB_PASSWORD=YourPassword

# JWT
JWT_SECRET_KEY=your-super-secret-key-at-least-32-characters-long
JWT_ISSUER=LightMarket
JWT_AUDIENCE=LightMarketUsers
JWT_EXPIRY_MINUTES=1440

# SMS
SMS_PROVIDER_API_KEY=your-sms-provider-api-key
SMS_SENDER_NUMBER=10001000
SMS_ENABLED=true

# Payment Gateway
PAYMENT_MERCHANT_ID=your-merchant-id
PAYMENT_API_KEY=your-payment-gateway-api-key
PAYMENT_CALLBACK_URL=https://yourdomain.com/api/payment/verify
PAYMENT_SANDBOX_MODE=true
```

## Security Notes

1. **Never commit sensitive data** to version control
2. Use **Azure Key Vault**, **AWS Secrets Manager**, or similar for production
3. Rotate secrets regularly
4. Use different keys for development and production
5. Enable HTTPS in production
