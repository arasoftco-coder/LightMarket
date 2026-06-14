using Microsoft.AspNetCore.Mvc;
using LampEcommerce.Application.Interfaces;

namespace LampEcommerce.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController : ControllerBase
{
    private readonly ICampaignService _campaignService;
    private readonly ILogger<CampaignController> _logger;

    public CampaignController(ICampaignService campaignService, ILogger<CampaignController> logger)
    {
        _campaignService = campaignService;
        _logger = logger;
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveCampaign()
    {
        try
        {
            var campaign = await _campaignService.GetActiveCampaign();
            return Ok(new { success = true, data = campaign });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active campaign");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetCampaignBySlug(string slug)
    {
        try
        {
            var campaign = await _campaignService.GetCampaignBySlug(slug);
            if (campaign == null)
                return NotFound(new { success = false, message = "Campaign not found" });
            return Ok(new { success = true, data = campaign });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting campaign by slug");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{id}/products")]
    public async Task<IActionResult> GetCampaignProducts(int id)
    {
        try
        {
            var products = await _campaignService.GetCampaignProducts(id);
            return Ok(new { success = true, data = products });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting campaign products");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("{id}/validate-access")]
    public async Task<IActionResult> ValidateCampaignAccess(int id)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { success = false, message = "User not authenticated" });

            var canAccess = await _campaignService.ValidateCampaignAccess(id, int.Parse(userId));
            return Ok(new { success = true, canAccess = canAccess });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating campaign access");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}
