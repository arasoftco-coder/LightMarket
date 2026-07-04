using System.Collections.Generic;
using System.Threading.Tasks;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Domain.Entities;
using LampEcommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LampEcommerce.Infrastructure.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly ApplicationDbContext _context;

    public SupplierRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Supplier?> GetByIdAsync(int id)
    {
        return _context.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Supplier>> GetAllAsync()
    {
        return await _context.Suppliers.ToListAsync();
    }

    public async Task<Supplier> AddAsync(Supplier supplier)
    {
        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();
        return supplier;
    }

    public async Task UpdateAsync(Supplier supplier)
    {
        _context.Suppliers.Update(supplier);
        await _context.SaveChangesAsync();
    }
}
