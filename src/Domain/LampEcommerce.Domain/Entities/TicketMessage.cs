using System;

namespace LampEcommerce.Domain.Entities;

public class TicketMessage
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public int SenderUserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Ticket? Ticket { get; set; }
    public User? SenderUser { get; set; }
}
