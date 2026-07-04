using System.Collections.Generic;
using System.Threading.Tasks;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Domain.Entities;
using LampEcommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LampEcommerce.Infrastructure.Repositories;

public class CampaignProductRepository : ICampaignProductRepository
{
    private readonly ApplicationDbContext _context;

    public CampaignProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<CampaignProduct?> GetByIdAsync(int id)
    {
        return _context.CampaignProducts
            .Include(cp => cp.Product)
            .Include(cp => cp.Campaign)
            .FirstOrDefaultAsync(cp => cp.Id == id);
    }

    public Task<CampaignProduct?> GetCampaignProductAsync(int campaignId, int productId)
    {
        return _context.CampaignProducts
            .Include(cp => cp.Product)
            .Include(cp => cp.Campaign)
            .FirstOrDefaultAsync(cp => cp.CampaignId == campaignId && cp.ProductId == productId);
    }

    public async Task<IEnumerable<CampaignProduct>> GetProductsByCampaignIdAsync(int campaignId)
    {
        return await _context.CampaignProducts
            .Include(cp => cp.Product)
            .Include(cp => cp.Campaign)
            .Where(cp => cp.CampaignId == campaignId)
            .ToListAsync();
    }

    public async Task<CampaignProduct> AddAsync(CampaignProduct campaignProduct)
    {
        _context.CampaignProducts.Add(campaignProduct);
        await _context.SaveChangesAsync();
        return campaignProduct;
    }

    public async Task UpdateAsync(CampaignProduct campaignProduct)
    {
        _context.CampaignProducts.Update(campaignProduct);
        await _context.SaveChangesAsync();
    }
}
