using System;
using System.Collections.Generic;

namespace LampEcommerce.Domain.Entities;

public class Ticket
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Category { get; set; } = string.Empty; // e.g. Financial, Delivery, Technical, Feedback
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = "Open"; // Open, InProgress, Closed
    public string Priority { get; set; } = "Low";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public User? User { get; set; }
    public ICollection<TicketMessage> Messages { get; set; } = new List<TicketMessage>();
}
