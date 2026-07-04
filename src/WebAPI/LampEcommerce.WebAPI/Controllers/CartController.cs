using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LampEcommerce.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ICampaignService _campaignService;
    private readonly ILogger<CartController> _logger;

    public CartController(
        ICartService cartService,
        ICampaignService campaignService,
        ILogger<CartController> logger)
    {
        _cartService = cartService;
        _campaignService = campaignService;
        _logger = logger;
    }

    private int GetUserId() => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        try
        {
            var userId = GetUserId();
            var activeCampaign = await _campaignService.GetActiveCampaign();
            if (activeCampaign == null)
            {
                return Ok(new { success = true, data = new CartDto { UserId = userId, Items = new List<CartItemDto>() } });
            }

            var cart = await _cartService.GetCart(userId, activeCampaign.Id);
            return Ok(new { success = true, data = cart });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        try
        {
            var userId = GetUserId();
            var activeCampaign = await _campaignService.GetActiveCampaign();
            if (activeCampaign == null)
            {
                return BadRequest(new { success = false, message = "هیچ کمپین فعالی برای خرید یافت نشد." });
            }

            var cartItem = await _cartService.AddToCart(userId, activeCampaign.Id, request.ProductId, request.Qty);
            return Ok(new { success = true, data = cartItem });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding to cart");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("{cartItemId}")]
    public async Task<IActionResult> UpdateQuantity(int cartItemId, [FromBody] UpdateQuantityRequest request)
    {
        try
        {
            var userId = GetUserId();
            var cartItem = await _cartService.UpdateQuantity(userId, cartItemId, request.Qty);
            return Ok(new { success = true, data = cartItem });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cart quantity");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpDelete("{cartItemId}")]
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

public class AddToCartRequest { public int ProductId { get; set; } public int Qty { get; set; } }
public class UpdateQuantityRequest { public int Qty { get; set; } }
