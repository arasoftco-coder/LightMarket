using System.Collections.Generic;
using System.Threading.Tasks;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Domain.Entities;
using LampEcommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LampEcommerce.Infrastructure.Repositories;

public class ShippingMethodRepository : IShippingMethodRepository
{
    private readonly ApplicationDbContext _context;

    public ShippingMethodRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<ShippingMethod?> GetByIdAsync(int id)
    {
        return _context.Set<ShippingMethod>().FirstOrDefaultAsync(sm => sm.Id == id);
    }

    public async Task<IEnumerable<ShippingMethod>> GetAllAsync()
    {
        return await _context.Set<ShippingMethod>().ToListAsync();
    }

    public async Task<ShippingMethod> AddAsync(ShippingMethod method)
    {
        _context.Set<ShippingMethod>().Add(method);
        await _context.SaveChangesAsync();
        return method;
    }

    public async Task UpdateAsync(ShippingMethod method)
    {
        _context.Set<ShippingMethod>().Update(method);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var method = await GetByIdAsync(id);
        if (method != null)
        {
            _context.Set<ShippingMethod>().Remove(method);
            await _context.SaveChangesAsync();
        }
    }
}
