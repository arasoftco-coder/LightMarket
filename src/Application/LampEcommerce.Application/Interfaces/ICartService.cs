using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Models;

namespace LampEcommerce.Application.Interfaces;

public interface ICartService
{
    Task<ApiResponse> AddToCartAsync(int userId, AddToCartRequest request);
    Task<ApiResponse> UpdateQuantityAsync(int userId, UpdateCartQuantityRequest request);
    Task<ApiResponse> RemoveFromCartAsync(int userId, int cartItemId);
    Task<IEnumerable<OrderItemDto>> GetCartAsync(int userId);
    Task<ApiResponse> ClearCartAsync(int userId);
}
