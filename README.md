# LightMarket - E-Commerce Platform

A modern, full-stack e-commerce platform built with .NET 8 WebAPI and Angular 17+.

## Tech Stack

### Backend
- .NET 8 WebAPI
- Entity Framework Core
- SQL Server
- JWT Authentication

### Frontend
- Angular 17+ (Standalone Components)
- PrimeNG UI Library
- TypeScript
- RxJS

## Features

- User authentication with OTP and JWT
- Campaign-based product sales
- Shopping cart management
- Multi-step checkout process
- Order tracking
- Support tickets system
- Magic link payments (for bot integration)
- Admin panel for campaign and product management
- Excel product import with fuzzy matching
- Web scraping for price updates

## Setup Instructions

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- SQL Server
- Git

### Backend Setup

1. Clone the repository
2. Update `appsettings.json` with your connection strings and API keys
3. Run migrations: `dotnet ef database update`
4. Start the API: `dotnet run --project src/LightMarket.WebAPI`

### Frontend Setup

1. Navigate to the Angular project: `cd src/app`
2. Install dependencies: `npm install`
3. Configure environment variables in `src/environments/environment.ts`
4. Start the development server: `ng serve`

## API Endpoints

### Authentication
- POST `/api/auth/send-otp` - Send OTP to phone number
- POST `/api/auth/verify-otp` - Verify OTP and get JWT token
- POST `/api/auth/register` - Register new user
- POST `/api/auth/set-password` - Set permanent password

### Campaigns
- GET `/api/campaigns/active` - Get active campaign
- GET `/api/campaigns/{slug}` - Get campaign by slug
- GET `/api/campaigns/{id}/products` - Get campaign products

### Cart
- GET `/api/cart/{campaignId}` - Get user's cart
- POST `/api/cart/add` - Add item to cart
- PUT `/api/cart/update/{cartItemId}` - Update quantity
- DELETE `/api/cart/remove/{cartItemId}` - Remove from cart

### Orders
- POST `/api/orders/create` - Create new order
- POST `/api/orders/{orderId}/confirm-payment` - Confirm payment (Admin)
- PUT `/api/orders/{orderId}/edit-invoice` - Edit invoice (Admin)
- GET `/api/orders/user/{userId}` - Get user's orders

### Users
- GET `/api/users/{userId}/profile` - Get profile
- PUT `/api/users/{userId}/profile` - Update profile
- GET `/api/users/{userId}/addresses` - Get addresses
- POST `/api/users/{userId}/addresses` - Add address

### Tickets
- POST `/api/tickets/create` - Create ticket
- GET `/api/tickets/user/{userId}` - Get user's tickets
- POST `/api/tickets/{ticketId}/reply` - Reply to ticket

## Environment Variables

See `ENVIRONMENT.md` for a complete list of required environment variables.

## Deployment

### Docker

Build and run with Docker Compose:
```bash
docker-compose up -d
```

### Manual Deployment

1. Build the backend: `dotnet publish -c Release`
2. Build the frontend: `ng build --configuration production`
3. Deploy to your hosting provider
4. Configure environment variables
5. Run database migrations

## Testing

See `TESTING.md` for manual test cases and testing guidelines.

## License

MIT License
