using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Application.Models;

namespace LampEcommerce.Application.Services;

public class CampaignService : ICampaignService
{
    public Task<IEnumerable<CampaignDto>> GetActiveCampaignsAsync()
    {
        // Return default active campaign
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

    public Task<CampaignDto?> GetCampaignBySlugAsync(string slug)
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

    public Task<CampaignDto?> GetCampaignByIdAsync(int campaignId)
    {
        var campaign = new CampaignDto
        {
            Id = campaignId,
            Name = $"Campaign {campaignId}",
            SupplierId = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(3),
            IsActive = true,
            IsDefault = false,
            MinOrderQty = 1,
            MaxOrderQty = 100
        };
        return Task.FromResult<CampaignDto?>(campaign);
    }

    public Task<IEnumerable<CampaignProductDto>> GetCampaignProductsAsync(int campaignId)
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
        return Task.FromResult<IEnumerable<CampaignProductDto>>(products);
    }

    public Task<bool> ValidateCampaignAccessAsync(int campaignId, int userId)
    {
        // Check if user can access this campaign (geographic restrictions, quantity limits)
        // In Phase 3, this will check database for user location and order history
        return Task.FromResult(true);
    }
}
