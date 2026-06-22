using LampEcommerce.Domain.Entities;

namespace LampEcommerce.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByPhoneNumberAsync(string phoneNumber);
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
}
