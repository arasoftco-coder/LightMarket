using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Models;

namespace LampEcommerce.Application.Services;

public class CartService : ICartService
{
    public Task<ApiResponse> AddToCartAsync(int userId, AddToCartRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse> UpdateQuantityAsync(int userId, UpdateCartQuantityRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse> RemoveFromCartAsync(int userId, int cartItemId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<OrderItemDto>> GetCartAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse> ClearCartAsync(int userId)
    {
        throw new NotImplementedException();
    }
}
