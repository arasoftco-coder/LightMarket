using System.Collections.Generic;
using System.Threading.Tasks;
using LampEcommerce.Domain.Entities;

namespace LampEcommerce.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByIdWithDetailsAsync(int id);
    Task<User?> GetByPhoneNumberAsync(string phoneNumber);
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
    Task<IEnumerable<User>> GetAllAsync();
    Task DeleteAddressAsync(int addressId);
    Task<Address?> GetAddressByIdAsync(int addressId);
    Task UpdateAddressAsync(Address address);
}
