using LampEcommerce.Application.DTOs;

namespace LampEcommerce.Application.Services;

public class CampaignService : ICampaignService
{
    public Task<IEnumerable<CampaignDto>> GetActiveCampaignsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<CampaignDto?> GetCampaignBySlugAsync(string slug)
    {
        throw new NotImplementedException();
    }

    public Task<CampaignDto?> GetCampaignByIdAsync(int campaignId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<CampaignProductDto>> GetCampaignProductsAsync(int campaignId)
    {
        throw new NotImplementedException();
    }
}
