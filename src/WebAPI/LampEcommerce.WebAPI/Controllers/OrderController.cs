using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Application.DTOs;

namespace LampEcommerce.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IOrderService orderService, ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    private int GetUserId() => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
    private bool IsAdmin() => User.IsInRole("Admin");

    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            var userId = GetUserId();
            var order = await _orderService.CreateOrder(userId, request.CartId, request.AddressId, request.ShippingMethod, request.PaymentMethod);
            return Ok(new { success = true, data = order });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("{orderId}/confirm-payment")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ConfirmPayment(int orderId, [FromBody] ConfirmPaymentRequest request)
    {
        try
        {
            var order = await _orderService.ConfirmPayment(orderId, request.TrackingCode);
            return Ok(new { success = true, data = order });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming payment");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("{orderId}/edit-invoice")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditInvoice(int orderId, [FromBody] EditInvoiceRequest request)
    {
        try
        {
            var userId = GetUserId();
            var order = await _orderService.EditInvoice(orderId, request.Changes, request.Reason, userId.ToString());
            return Ok(new { success = true, data = order });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing invoice");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("{orderId}/update-shipping")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateShippingInfo(int orderId, [FromBody] UpdateShippingRequest request)
    {
        try
        {
            var order = await _orderService.UpdateShippingInfo(orderId, request.TrackingCode, request.ShippingCompany);
            return Ok(new { success = true, data = order });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating shipping info");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserOrders(int userId)
    {
        try
        {
            var currentUserId = GetUserId();
            if (currentUserId != userId && !IsAdmin())
                return Forbid();

            var orders = await _orderService.GetUserOrders(userId);
            return Ok(new { success = true, data = orders });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user orders");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrderDetails(int orderId)
    {
        try
        {
            var order = await _orderService.GetOrderDetails(orderId);
            if (order == null)
                return NotFound(new { success = false, message = "Order not found" });
            return Ok(new { success = true, data = order });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order details");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}

public class CreateOrderRequest { public int CartId { get; set; } public int AddressId { get; set; } public string ShippingMethod { get; set; } = string.Empty; public string PaymentMethod { get; set; } = string.Empty; }
public class ConfirmPaymentRequest { public string TrackingCode { get; set; } = string.Empty; }
public class EditInvoiceRequest { public Dictionary<string, object> Changes { get; set; } = new(); public string Reason { get; set; } = string.Empty; }
public class UpdateShippingRequest { public string TrackingCode { get; set; } = string.Empty; public string ShippingCompany { get; set; } = string.Empty; }
