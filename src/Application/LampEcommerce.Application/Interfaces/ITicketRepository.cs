using System.Collections.Generic;
using System.Threading.Tasks;
using LampEcommerce.Domain.Entities;

namespace LampEcommerce.Application.Interfaces;

public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(int id);
    Task<Ticket?> GetByIdWithMessagesAsync(int id);
    Task<IEnumerable<Ticket>> GetAllAsync();
    Task<IEnumerable<Ticket>> GetByUserIdAsync(int userId);
    Task<Ticket> AddAsync(Ticket ticket);
    Task UpdateAsync(Ticket ticket);
    Task<TicketMessage> AddMessageAsync(TicketMessage message);
}
