using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Interfaces;

namespace LampEcommerce.Application.Services;

public class TicketService : ITicketService
{
    public Task<TicketDto?> CreateTicket(int userId, string category, string subject, string message)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TicketDto>> GetUserTickets(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<TicketDto?> GetTicketDetails(int ticketId)
    {
        throw new NotImplementedException();
    }

    public Task<TicketDto?> ReplyToTicket(int ticketId, int userId, string message)
    {
        throw new NotImplementedException();
    }
}
