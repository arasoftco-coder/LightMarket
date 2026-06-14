using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Application.DTOs;

namespace LampEcommerce.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly ILogger<TicketController> _logger;

    public TicketController(ITicketService ticketService, ILogger<TicketController> logger)
    {
        _ticketService = ticketService;
        _logger = logger;
    }

    private int GetUserId() => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpPost("create")]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest request)
    {
        try
        {
            var userId = GetUserId();
            var ticket = await _ticketService.CreateTicket(userId, request.Category, request.Subject, request.Message);
            return Ok(new { success = true, data = ticket });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating ticket");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserTickets(int userId)
    {
        try
        {
            var currentUserId = GetUserId();
            if (currentUserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var tickets = await _ticketService.GetUserTickets(userId);
            return Ok(new { success = true, data = tickets });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user tickets");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{ticketId}")]
    public async Task<IActionResult> GetTicketDetails(int ticketId)
    {
        try
        {
            var ticket = await _ticketService.GetTicketDetails(ticketId);
            if (ticket == null)
                return NotFound(new { success = false, message = "Ticket not found" });
            return Ok(new { success = true, data = ticket });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ticket details");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("{ticketId}/reply")]
    public async Task<IActionResult> ReplyToTicket(int ticketId, [FromBody] ReplyTicketRequest request)
    {
        try
        {
            var userId = GetUserId();
            var ticket = await _ticketService.ReplyToTicket(ticketId, userId, request.Message);
            return Ok(new { success = true, data = ticket });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error replying to ticket");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}

public class CreateTicketRequest { public string Category { get; set; } = string.Empty; public string Subject { get; set; } = string.Empty; public string Message { get; set; } = string.Empty; }
public class ReplyTicketRequest { public string Message { get; set; } = string.Empty; }
