using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Application.DTOs;

namespace LampEcommerce.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    private int GetUserId() => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpGet("{userId}/profile")]
    public async Task<IActionResult> GetUserProfile(int userId)
    {
        try
        {
            var currentUserId = GetUserId();
            if (currentUserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var user = await _userService.GetUserProfile(userId);
            if (user == null)
                return NotFound(new { success = false, message = "User not found" });
            return Ok(new { success = true, data = user });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("{userId}/profile")]
    public async Task<IActionResult> UpdateUserProfile(int userId, [FromBody] UpdateUserRequest request)
    {
        try
        {
            var currentUserId = GetUserId();
            if (currentUserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var user = await _userService.UpdateUserProfile(userId, request.FullName, request.Email);
            return Ok(new { success = true, data = user });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{userId}/addresses")]
    public async Task<IActionResult> GetUserAddresses(int userId)
    {
        try
        {
            var currentUserId = GetUserId();
            if (currentUserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var addresses = await _userService.GetUserAddresses(userId);
            return Ok(new { success = true, data = addresses });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user addresses");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("{userId}/addresses")]
    public async Task<IActionResult> AddAddress(int userId, [FromBody] AddAddressRequest request)
    {
        try
        {
            var currentUserId = GetUserId();
            if (currentUserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var address = await _userService.AddAddress(userId, request.FullAddress, request.City, request.PostalCode, request.Phone);
            return Ok(new { success = true, data = address });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding address");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("addresses/{addressId}")]
    public async Task<IActionResult> UpdateAddress(int addressId, [FromBody] UpdateAddressRequest request)
    {
        try
        {
            var address = await _userService.UpdateAddress(addressId, request.FullAddress, request.City, request.PostalCode, request.Phone);
            return Ok(new { success = true, data = address });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating address");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpDelete("addresses/{addressId}")]
    public async Task<IActionResult> DeleteAddress(int addressId)
    {
        try
        {
            await _userService.DeleteAddress(addressId);
            return Ok(new { success = true, message = "Address deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting address");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}

public class UpdateUserRequest { public string FullName { get; set; } = string.Empty; public string Email { get; set; } = string.Empty; }
public class AddAddressRequest { public string FullAddress { get; set; } = string.Empty; public string City { get; set; } = string.Empty; public string PostalCode { get; set; } = string.Empty; public string Phone { get; set; } = string.Empty; }
public class UpdateAddressRequest { public string FullAddress { get; set; } = string.Empty; public string City { get; set; } = string.Empty; public string PostalCode { get; set; } = string.Empty; public string Phone { get; set; } = string.Empty; }
