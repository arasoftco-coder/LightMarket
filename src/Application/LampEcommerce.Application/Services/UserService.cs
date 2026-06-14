using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Interfaces;

namespace LampEcommerce.Application.Services;

public class UserService : IUserService
{
    public Task<UserDto?> GetUserProfile(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<UserDto?> UpdateUserProfile(int userId, string fullName, string email)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AddressDto>> GetUserAddresses(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<AddressDto?> AddAddress(int userId, string fullAddress, string city, string postalCode, string phone)
    {
        throw new NotImplementedException();
    }

    public Task<AddressDto?> UpdateAddress(int addressId, string fullAddress, string city, string postalCode, string phone)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAddress(int addressId)
    {
        throw new NotImplementedException();
    }
}
