using System.Collections.Generic;
using System.Threading.Tasks;
using LampEcommerce.Domain.Entities;

namespace LampEcommerce.Application.Interfaces;

public interface ICampaignProductRepository
{
    Task<CampaignProduct?> GetByIdAsync(int id);
    Task<CampaignProduct?> GetCampaignProductAsync(int campaignId, int productId);
    Task<IEnumerable<CampaignProduct>> GetProductsByCampaignIdAsync(int campaignId);
    Task<CampaignProduct> AddAsync(CampaignProduct campaignProduct);
    Task UpdateAsync(CampaignProduct campaignProduct);
}
