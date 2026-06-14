using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LampEcommerce.Application.Interfaces;

namespace LampEcommerce.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly ICampaignService _campaignService;
    private readonly IScraperService _scraperService;
    private readonly IFuzzyMatchingService _fuzzyMatchingService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        ICampaignService campaignService,
        IScraperService scraperService,
        IFuzzyMatchingService fuzzyMatchingService,
        ILogger<AdminController> logger)
    {
        _campaignService = campaignService;
        _scraperService = scraperService;
        _fuzzyMatchingService = fuzzyMatchingService;
        _logger = logger;
    }

    [HttpGet("campaigns")]
    public async Task<IActionResult> GetAllCampaigns()
    {
        try
        {
            var campaigns = await _campaignService.GetAllCampaigns();
            return Ok(new { success = true, data = campaigns });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all campaigns");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("campaigns")]
    public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignRequest request)
    {
        try
        {
            var campaign = await _campaignService.CreateCampaign(request.Name, request.Slug, request.StartDate, request.EndDate, request.IsActive);
            return Ok(new { success = true, data = campaign });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating campaign");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("campaigns/{id}")]
    public async Task<IActionResult> UpdateCampaign(int id, [FromBody] UpdateCampaignRequest request)
    {
        try
        {
            var campaign = await _campaignService.UpdateCampaign(id, request.Name, request.Slug, request.StartDate, request.EndDate, request.IsActive);
            return Ok(new { success = true, data = campaign });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating campaign");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("campaigns/{id}/report")]
    public async Task<IActionResult> GetCampaignReport(int id)
    {
        try
        {
            var report = await _campaignService.GetCampaignReport(id);
            return Ok(new { success = true, data = report });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting campaign report");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("suppliers")]
    public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierRequest request)
    {
        try
        {
            // Placeholder for supplier creation
            return Ok(new { success = true, message = "Supplier created successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating supplier");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("products/import-excel")]
    public async Task<IActionResult> ImportProductsFromExcel(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { success = false, message = "No file uploaded" });

            // Placeholder for Excel import with fuzzy matching
            return Ok(new { success = true, message = "Products imported successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing products from Excel");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("products/scrape/{supplierId}")]
    public async Task<IActionResult> ScrapeProducts(int supplierId, [FromBody] ScrapeRequest request)
    {
        try
        {
            var products = await _scraperService.ScrapeFromUrl(supplierId, request.Url, request.ExtractionConfig);
            return Ok(new { success = true, data = products });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scraping products");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}

public class CreateCampaignRequest { public string Name { get; set; } = string.Empty; public string Slug { get; set; } = string.Empty; public DateTime StartDate { get; set; } public DateTime EndDate { get; set; } public bool IsActive { get; set; } }
public class UpdateCampaignRequest { public string Name { get; set; } = string.Empty; public string Slug { get; set; } = string.Empty; public DateTime StartDate { get; set; } public DateTime EndDate { get; set; } public bool IsActive { get; set; } }
public class CreateSupplierRequest { public string Name { get; set; } = string.Empty; public string WebsiteUrl { get; set; } = string.Empty; }
public class ScrapeRequest { public string Url { get; set; } = string.Empty; public Dictionary<string, string> ExtractionConfig { get; set; } = new(); }
