using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Domain.Entities;

namespace LampEcommerce.Application.Services;

public class CampaignService(
    ICampaignRepository campaignRepository,
    ICampaignProductRepository campaignProductRepository,
    IUserRepository userRepository,
    ISupplierRepository supplierRepository) : ICampaignService
{
    private readonly ICampaignRepository _campaignRepository = campaignRepository;
    private readonly ICampaignProductRepository _campaignProductRepository = campaignProductRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ISupplierRepository _supplierRepository = supplierRepository;


    public async Task<CampaignDto?> GetActiveCampaign()
    {
        var campaign = await _campaignRepository.GetDefaultActiveCampaignAsync();
        if (campaign == null) return null;

        if (campaign.EndDate < DateTime.UtcNow)
        {
            throw new InvalidOperationException("کمپین پیش‌فرض فعال منقضی شده است.");
        }

        return MapToDto(campaign);
    }

    public async Task<CampaignDto?> GetCampaignBySlug(string slug)
    {
        var campaign = await _campaignRepository.GetBySlugAsync(slug);
        if (campaign == null) return null;

        if (!campaign.IsActive)
        {
            throw new InvalidOperationException("این کمپین در حال حاضر فعال نیست.");
        }
        if (campaign.EndDate < DateTime.UtcNow)
        {
            throw new InvalidOperationException("این کمپین منقضی شده است.");
        }

        return MapToDto(campaign);
    }

    public async Task<List<CampaignProductDto>> GetCampaignProducts(int campaignId)
    {
        var campaignProducts = await _campaignProductRepository.GetProductsByCampaignIdAsync(campaignId);
        return campaignProducts.Select(cp => new CampaignProductDto
        {
            Id = cp.Id,
            CampaignId = cp.CampaignId,
            ProductId = cp.ProductId,
            PurchasePrice = cp.PurchasePrice,
            SellingPrice = cp.SellingPrice,
            Discount = cp.Discount,
            Stock = cp.Stock,
            MinQtyPerUser = cp.MinQtyPerUser,
            MaxQtyPerUser = cp.MaxQtyPerUser,
            Product = cp.Product == null ? null : new ProductDto
            {
                Id = cp.Product.Id,
                Name = cp.Product.Name,
                ImageUrl = cp.Product.ImageUrl,
                Description = cp.Product.Description,
                BasePrice = cp.Product.BasePrice
            }
        }).ToList();
    }

    public async Task<bool> ValidateCampaignAccess(int campaignId, int userId)
    {
        var campaign = await _campaignRepository.GetByIdAsync(campaignId);
        if (campaign == null) return false;

        // 1. Check active and date bounds
        if (!campaign.IsActive || campaign.StartDate > DateTime.UtcNow || campaign.EndDate < DateTime.UtcNow)
        {
            return false;
        }

        var user = await _userRepository.GetByIdWithDetailsAsync(userId);
        if (user == null) return false;

        // 2. Validate geographic restrictions (Provinces / Cities)
        if (!string.IsNullOrWhiteSpace(campaign.AllowedProvinces) || !string.IsNullOrWhiteSpace(campaign.AllowedCities))
        {
            var allowedProvincesList = campaign.AllowedProvinces?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim().ToLower())
                .ToList() ?? [];

            var allowedCitiesList = campaign.AllowedCities?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.Trim().ToLower())
                .ToList() ?? [];

            var matchesGeo = user.Addresses.Any(addr =>
                (allowedProvincesList.Count == 0 || allowedProvincesList.Contains(addr.Province.Trim().ToLower())) &&
                (allowedCitiesList.Count == 0 || allowedCitiesList.Contains(addr.City.Trim().ToLower()))
            );

            if (!matchesGeo) return false;
        }

        // 3. Validate user total order quantity limits in this campaign (from completed orders)
        if (campaign.MaxOrderQty > 0 || campaign.MinOrderQty > 0)
        {
            var totalOrderedQty = user.Orders
                .Where(o => o.CampaignId == campaignId && o.Status != "Open" && o.Status != "Cancelled")
                .SelectMany(o => o.OrderItems)
                .Sum(oi => oi.Quantity);

            if (campaign.MaxOrderQty > 0 && totalOrderedQty >= campaign.MaxOrderQty)
            {
                return false;
            }
        }

        return true;
    }

    public async Task<IEnumerable<CampaignDto>> GetAllCampaigns()
    {
        var campaigns = await _campaignRepository.GetAllAsync();
        return campaigns.Select(MapToDto);
    }

    public async Task<CampaignDto?> CreateCampaign(string name, string slug, DateTime startDate, DateTime endDate, bool isActive)
    {
        var suppliers = await _supplierRepository.GetAllAsync();
        var supplier = suppliers.FirstOrDefault();
        if (supplier == null)
        {
            supplier = new Supplier
            {
                Name = "Default Supplier",
                Website = "https://defaultsupplier.com",
                ContactInfo = "info@defaultsupplier.com"
            };
            supplier = await _supplierRepository.AddAsync(supplier);
        }

        var campaign = new Campaign
        {
            Name = name,
            Slug = slug,
            SupplierId = supplier.Id,
            StartDate = startDate,
            EndDate = endDate,
            IsActive = isActive,
            IsDefault = !suppliers.Any(), // If it's the first campaign, make it default
            MinOrderQty = 1,
            MaxOrderQty = 100
        };

        var created = await _campaignRepository.AddAsync(campaign);
        return MapToDto(created);
    }

    public async Task<CampaignDto?> UpdateCampaign(int id, string name, string slug, DateTime startDate, DateTime endDate, bool isActive)
    {
        var campaign = await _campaignRepository.GetByIdAsync(id);
        if (campaign == null) return null;

        campaign.Name = name;
        campaign.Slug = slug;
        campaign.StartDate = startDate;
        campaign.EndDate = endDate;
        campaign.IsActive = isActive;

        await _campaignRepository.UpdateAsync(campaign);
        return MapToDto(campaign);
    }

    public async Task<CampaignDto?> GetCampaignById(int id)
    {
        var campaign = await _campaignRepository.GetByIdAsync(id);
        return campaign == null ? null : MapToDto(campaign);
    }

    public async Task<object?> GetCampaignReport(int campaignId)
    {
        var campaign = await _campaignRepository.GetByIdAsync(campaignId);
        if (campaign == null) return null;

        return new
        {
            CampaignId = campaignId,
            CampaignName = campaign.Name,
            TotalOrders = 0,
            TotalRevenue = 0.00m,
            TotalUsers = 0
        };
    }

    private static CampaignDto MapToDto(Campaign campaign)
    {
        return new CampaignDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Slug = campaign.Slug,
            SupplierId = campaign.SupplierId,
            StartDate = campaign.StartDate,
            EndDate = campaign.EndDate,
            IsActive = campaign.IsActive,
            IsDefault = campaign.IsDefault,
            AllowedProvinces = campaign.AllowedProvinces,
            AllowedCities = campaign.AllowedCities,
            MinOrderQty = campaign.MinOrderQty,
            MaxOrderQty = campaign.MaxOrderQty
        };
    }
}
