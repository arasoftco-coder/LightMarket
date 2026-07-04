using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Domain.Entities;
using LampEcommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LampEcommerce.Infrastructure.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly ApplicationDbContext _context;

    public TicketRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Ticket?> GetByIdAsync(int id)
    {
        return _context.Set<Ticket>().FirstOrDefaultAsync(t => t.Id == id);
    }

    public Task<Ticket?> GetByIdWithMessagesAsync(int id)
    {
        return _context.Set<Ticket>()
            .Include(t => t.Messages)
                .ThenInclude(m => m.SenderUser)
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Ticket>> GetAllAsync()
    {
        return await _context.Set<Ticket>()
            .Include(t => t.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetByUserIdAsync(int userId)
    {
        return await _context.Set<Ticket>()
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    public async Task<Ticket> AddAsync(Ticket ticket)
    {
        _context.Set<Ticket>().Add(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }

    public async Task UpdateAsync(Ticket ticket)
    {
        _context.Set<Ticket>().Update(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task<TicketMessage> AddMessageAsync(TicketMessage message)
    {
        _context.Set<TicketMessage>().Add(message);
        await _context.SaveChangesAsync();
        return message;
    }
}
