using LampEcommerce.Application.DTOs;

namespace LampEcommerce.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserProfile(int userId);
    Task<UserDto?> UpdateUserProfile(int userId, string fullName, string email);
    Task<IEnumerable<AddressDto>> GetUserAddresses(int userId);
    Task<AddressDto?> AddAddress(int userId, string fullAddress, string city, string postalCode, string phone);
    Task<AddressDto?> UpdateAddress(int addressId, string fullAddress, string city, string postalCode, string phone);
    Task<bool> DeleteAddress(int addressId);
}
