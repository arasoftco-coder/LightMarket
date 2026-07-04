using System.Collections.Generic;
using System.Threading.Tasks;
using LampEcommerce.Domain.Entities;

namespace LampEcommerce.Application.Interfaces;

public interface ICampaignRepository
{
    Task<Campaign?> GetByIdAsync(int id);
    Task<Campaign?> GetBySlugAsync(string slug);
    Task<Campaign?> GetDefaultActiveCampaignAsync();
    Task<IEnumerable<Campaign>> GetAllAsync();
    Task<Campaign> AddAsync(Campaign campaign);
    Task UpdateAsync(Campaign campaign);
}
