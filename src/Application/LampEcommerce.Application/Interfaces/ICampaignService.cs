using LampEcommerce.Application.DTOs;

namespace LampEcommerce.Application.Interfaces;

public interface ICampaignService
{
    Task<CampaignDto?> GetActiveCampaign();
    Task<CampaignDto?> GetCampaignBySlug(string slug);
    Task<List<CampaignProductDto>> GetCampaignProducts(int campaignId);
    Task<bool> ValidateCampaignAccess(int campaignId, int userId);
    Task<IEnumerable<CampaignDto>> GetAllCampaigns();
    Task<CampaignDto?> CreateCampaign(string name, string slug, DateTime startDate, DateTime endDate, bool isActive);
    Task<CampaignDto?> UpdateCampaign(int id, string name, string slug, DateTime startDate, DateTime endDate, bool isActive);
    Task<CampaignDto?> GetCampaignById(int id);
    Task<object?> GetCampaignReport(int campaignId);
}
