using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Domain.Entities;
using LampEcommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LampEcommerce.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Order?> GetByIdAsync(int id)
    {
        return _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
    }

    public Task<Order?> GetByIdWithDetailsAsync(int id)
    {
        return _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.User)
            .Include(o => o.Campaign)
            .Include(o => o.InvoiceAuditLogs)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders.ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllWithDetailsAsync()
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.User)
            .Include(o => o.Campaign)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(int userId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.UserId == userId)
            .ToListAsync();
    }

    public async Task<Order> AddAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteOrderItemAsync(int orderItemId)
    {
        var item = await _context.OrderItems.FindAsync(orderItemId);
        if (item != null)
        {
            _context.OrderItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}
