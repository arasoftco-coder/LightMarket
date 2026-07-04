using LampEcommerce.Application.Interfaces;
using LampEcommerce.Domain.Entities;
using LampEcommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LampEcommerce.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByIdAsync(int id)
    {
        return _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public Task<User?> GetByIdWithDetailsAsync(int id)
    {
        return _context.Users
            .Include(u => u.Addresses)
            .Include(u => u.Orders)
                .ThenInclude(o => o.OrderItems)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public Task<User?> GetByPhoneNumberAsync(string phoneNumber)
    {
        return _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
    }

    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task DeleteAddressAsync(int addressId)
    {
        var address = await _context.Addresses.FindAsync(addressId);
        if (address != null)
        {
            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
        }
    }

    public Task<Address?> GetAddressByIdAsync(int addressId)
    {
        return _context.Addresses.FirstOrDefaultAsync(a => a.Id == addressId);
    }

    public async Task UpdateAddressAsync(Address address)
    {
        _context.Addresses.Update(address);
        await _context.SaveChangesAsync();
    }
}
