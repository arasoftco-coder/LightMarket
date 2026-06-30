namespace LampEcommerce.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string? PasswordHash { get; set; }
    public string Role { get; set; } = UserRoles.Customer;
    public string? BaleChatId { get; set; }
    public string? TelegramChatId { get; set; }
    public User() { }

    public User(string fullName, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty.", nameof(fullName));
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty.", nameof(phoneNumber));

        FullName = fullName;
        PhoneNumber = phoneNumber;
    }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
