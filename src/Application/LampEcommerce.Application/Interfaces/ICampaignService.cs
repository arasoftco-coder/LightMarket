using LampEcommerce.Application.DTOs;

namespace LampEcommerce.Application.Interfaces;

public interface ICampaignService
{
    Task<IEnumerable<CampaignDto>> GetActiveCampaignsAsync();
    Task<CampaignDto?> GetCampaignBySlugAsync(string slug);
    Task<CampaignDto?> GetCampaignByIdAsync(int campaignId);
    Task<IEnumerable<CampaignProductDto>> GetCampaignProductsAsync(int campaignId);
}
