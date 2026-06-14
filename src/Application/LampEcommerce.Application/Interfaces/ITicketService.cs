using LampEcommerce.Application.DTOs;

namespace LampEcommerce.Application.Interfaces;

public interface ITicketService
{
    Task<TicketDto?> CreateTicket(int userId, string category, string subject, string message);
    Task<IEnumerable<TicketDto>> GetUserTickets(int userId);
    Task<TicketDto?> GetTicketDetails(int ticketId);
    Task<TicketDto?> ReplyToTicket(int ticketId, int userId, string message);
}
