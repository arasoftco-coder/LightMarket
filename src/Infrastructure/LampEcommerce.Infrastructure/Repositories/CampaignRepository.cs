using System.Collections.Generic;
using System.Threading.Tasks;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Domain.Entities;
using LampEcommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LampEcommerce.Infrastructure.Repositories;

public class CampaignRepository : ICampaignRepository
{
    private readonly ApplicationDbContext _context;

    public CampaignRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Campaign?> GetByIdAsync(int id)
    {
        return _context.Campaigns.Include(c => c.Supplier).FirstOrDefaultAsync(c => c.Id == id);
    }

    public Task<Campaign?> GetBySlugAsync(string slug)
    {
        return _context.Campaigns.Include(c => c.Supplier).FirstOrDefaultAsync(c => c.Slug == slug);
    }

    public Task<Campaign?> GetDefaultActiveCampaignAsync()
    {
        return _context.Campaigns
            .Include(c => c.Supplier)
            .FirstOrDefaultAsync(c => c.IsDefault && c.IsActive);
    }

    public async Task<IEnumerable<Campaign>> GetAllAsync()
    {
        return await _context.Campaigns.Include(c => c.Supplier).ToListAsync();
    }

    public async Task<Campaign> AddAsync(Campaign campaign)
    {
        _context.Campaigns.Add(campaign);
        await _context.SaveChangesAsync();
        return campaign;
    }

    public async Task UpdateAsync(Campaign campaign)
    {
        _context.Campaigns.Update(campaign);
        await _context.SaveChangesAsync();
    }
}
