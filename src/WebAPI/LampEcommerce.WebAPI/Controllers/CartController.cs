using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Application.DTOs;

namespace LampEcommerce.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ILogger<CartController> _logger;

    public CartController(ICartService cartService, ILogger<CartController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    private int GetUserId() => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpGet("{campaignId}")]
    public async Task<IActionResult> GetCart(int campaignId)
    {
        try
        {
            var userId = GetUserId();
            var cart = await _cartService.GetCart(userId, campaignId);
            return Ok(new { success = true, data = cart });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        try
        {
            var userId = GetUserId();
            var cartItem = await _cartService.AddToCart(userId, request.CampaignId, request.ProductId, request.Quantity);
            return Ok(new { success = true, data = cartItem });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding to cart");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("update/{cartItemId}")]
    public async Task<IActionResult> UpdateQuantity(int cartItemId, [FromBody] UpdateQuantityRequest request)
    {
        try
        {
            var userId = GetUserId();
            var cartItem = await _cartService.UpdateQuantity(userId, cartItemId, request.Quantity);
            return Ok(new { success = true, data = cartItem });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cart quantity");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpDelete("remove/{cartItemId}")]
    public async Task<IActionResult> RemoveFromCart(int cartItemId)
    {
        try
        {
            var userId = GetUserId();
            await _cartService.RemoveFromCart(userId, cartItemId);
            return Ok(new { success = true, message = "Item removed from cart" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing from cart");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}

public class AddToCartRequest { public int CampaignId { get; set; } public int ProductId { get; set; } public int Quantity { get; set; } }
public class UpdateQuantityRequest { public int Quantity { get; set; } }
