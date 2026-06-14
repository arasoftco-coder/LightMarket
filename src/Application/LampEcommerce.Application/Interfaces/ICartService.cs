using LampEcommerce.Application.DTOs;

namespace LampEcommerce.Application.Interfaces;

public interface ICartService
{
    Task<CartItemDto?> AddToCart(int userId, int campaignId, int productId, int quantity);
    Task<CartItemDto?> UpdateQuantity(int userId, int cartItemId, int quantity);
    Task<bool> RemoveFromCart(int userId, int cartItemId);
    Task<CartDto?> GetCart(int userId, int campaignId);
}
