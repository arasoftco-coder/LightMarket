using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Application.Models;

namespace LampEcommerce.Application.Services;

public class CampaignService : ICampaignService
{
    public Task<CampaignDto?> GetActiveCampaign()
    {
        // Return default active campaign
        var campaign = new CampaignDto
        {
            Id = 1,
            Name = "Default Campaign",
            SupplierId = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(3),
            IsActive = true,
            IsDefault = true,
            MinOrderQty = 1,
            MaxOrderQty = 100
        };
        return Task.FromResult<CampaignDto?>(campaign);
    }

    public Task<CampaignDto?> GetCampaignBySlug(string slug)
    {
        // Return campaign by URL-friendly name
        var campaign = new CampaignDto
        {
            Id = 1,
            Name = "Default Campaign",
            Slug = slug,
            SupplierId = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(3),
            IsActive = true,
            IsDefault = true,
            MinOrderQty = 1,
            MaxOrderQty = 100
        };
        return Task.FromResult<CampaignDto?>(campaign);
    }

    public Task<List<CampaignProductDto>> GetCampaignProducts(int campaignId)
    {
        // Return products with prices, stock, and discounts
        var products = new List<CampaignProductDto>
        {
            new CampaignProductDto
            {
                Id = 1,
                CampaignId = campaignId,
                ProductId = 1,
                PurchasePrice = 100.00m,
                SellingPrice = 150.00m,
                Discount = 10.00m,
                Stock = 50,
                MinQtyPerUser = 1,
                MaxQtyPerUser = 10,
                Product = new ProductDto
                {
                    Id = 1,
                    Name = "Sample Product",
                    BasePrice = 100.00m
                }
            }
        };
        return Task.FromResult<List<CampaignProductDto>>(products);
    }

    public Task<bool> ValidateCampaignAccess(int campaignId, int userId)
    {
        // Check if user can access this campaign (geographic restrictions, quantity limits)
        // In Phase 3, this will check database for user location and order history
        return Task.FromResult(true);
    }

    public Task<IEnumerable<CampaignDto>> GetAllCampaigns()
    {
        var campaigns = new List<CampaignDto>
        {
            new CampaignDto
            {
                Id = 1,
                Name = "Default Campaign",
                SupplierId = 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(3),
                IsActive = true,
                IsDefault = true,
                MinOrderQty = 1,
                MaxOrderQty = 100
            }
        };
        return Task.FromResult<IEnumerable<CampaignDto>>(campaigns);
    }

    public Task<CampaignDto?> CreateCampaign(string name, string slug, DateTime startDate, DateTime endDate, bool isActive)
    {
        var campaign = new CampaignDto
        {
            Id = 1,
            Name = name,
            Slug = slug,
            SupplierId = 1,
            StartDate = startDate,
            EndDate = endDate,
            IsActive = isActive,
            IsDefault = false,
            MinOrderQty = 1,
            MaxOrderQty = 100
        };
        return Task.FromResult<CampaignDto?>(campaign);
    }

    public Task<CampaignDto?> UpdateCampaign(int id, string name, string slug, DateTime startDate, DateTime endDate, bool isActive)
    {
        var campaign = new CampaignDto
        {
            Id = id,
            Name = name,
            Slug = slug,
            SupplierId = 1,
            StartDate = startDate,
            EndDate = endDate,
            IsActive = isActive,
            IsDefault = false,
            MinOrderQty = 1,
            MaxOrderQty = 100
        };
        return Task.FromResult<CampaignDto?>(campaign);
    }

    public Task<object?> GetCampaignReport(int campaignId)
    {
        var report = new
        {
            CampaignId = campaignId,
            TotalOrders = 10,
            TotalRevenue = 5000.00m,
            TotalUsers = 5
        };
        return Task.FromResult<object?>(report);
    }
}
