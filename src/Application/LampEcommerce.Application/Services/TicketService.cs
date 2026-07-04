using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LampEcommerce.Application.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUserRepository _userRepository;

    public TicketService(ITicketRepository ticketRepository, IUserRepository userRepository)
    {
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;
    }

    public async Task<TicketDto?> CreateTicket(int userId, string category, string subject, string message)
    {
        var ticket = new Ticket
        {
            UserId = userId,
            Category = category,
            Subject = subject,
            Message = message,
            Status = "Open",
            Priority = "Medium",
            CreatedAt = DateTime.UtcNow
        };

        var created = await _ticketRepository.AddAsync(ticket);

        // Add initial message
        var initialMsg = new TicketMessage
        {
            TicketId = created.Id,
            SenderUserId = userId,
            Message = message,
            CreatedAt = DateTime.UtcNow
        };
        await _ticketRepository.AddMessageAsync(initialMsg);

        return await GetTicketDetails(created.Id);
    }

    public async Task<IEnumerable<TicketDto>> GetUserTickets(int userId)
    {
        var tickets = await _ticketRepository.GetByUserIdAsync(userId);
        return tickets.Select(MapToDto);
    }

    public async Task<TicketDto?> GetTicketDetails(int ticketId)
    {
        var ticket = await _ticketRepository.GetByIdWithMessagesAsync(ticketId);
        if (ticket == null) return null;

        var dto = MapToDto(ticket);
        dto.Messages = ticket.Messages.Select(m => new TicketMessageDto
        {
            Id = m.Id,
            Message = m.Message,
            Sender = (m.SenderUser != null && m.SenderUser.Role == UserRoles.Admin) ? "admin" : "user",
            CreatedAt = m.CreatedAt
        }).OrderBy(m => m.CreatedAt).ToList();

        return dto;
    }

    public async Task<TicketDto?> ReplyToTicket(int ticketId, int userId, string message)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null) return null;

        var msg = new TicketMessage
        {
            TicketId = ticketId,
            SenderUserId = userId,
            Message = message,
            CreatedAt = DateTime.UtcNow
        };
        await _ticketRepository.AddMessageAsync(msg);

        ticket.UpdatedAt = DateTime.UtcNow;
        if (userId != ticket.UserId)
        {
            // If replied by someone else (likely support staff or admin), change status to InProgress or Pending
            ticket.Status = "InProgress";
        }
        else
        {
            ticket.Status = "Open";
        }

        await _ticketRepository.UpdateAsync(ticket);

        return await GetTicketDetails(ticketId);
    }

    private static TicketDto MapToDto(Ticket t)
    {
        return new TicketDto
        {
            Id = t.Id,
            UserId = t.UserId,
            Category = t.Category,
            Subject = t.Subject,
            Message = t.Message,
            Status = t.Status,
            Priority = t.Priority,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            User = t.User == null ? null : new UserDto
            {
                Id = t.User.Id,
                FullName = t.User.FullName,
                PhoneNumber = t.User.PhoneNumber,
                Role = t.User.Role,
                Avatar = t.User.Avatar,
                CreatedAt = t.User.CreatedAt
            }
        };
    }
}
