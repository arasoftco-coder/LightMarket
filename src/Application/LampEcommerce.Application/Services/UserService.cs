using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LampEcommerce.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> GetUserProfile(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            Avatar = user.Avatar,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserDto?> UpdateUserProfile(int userId, string fullName, string email)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        user.FullName = fullName;

        await _userRepository.UpdateAsync(user);

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            Avatar = user.Avatar,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<IEnumerable<AddressDto>> GetUserAddresses(int userId)
    {
        var user = await _userRepository.GetByIdWithDetailsAsync(userId);
        if (user == null) return new List<AddressDto>();

        return user.Addresses.Select(MapAddressToDto);
    }

    public async Task<AddressDto?> AddAddress(int userId, string fullAddress, string city, string postalCode, string phone)
    {
        var user = await _userRepository.GetByIdWithDetailsAsync(userId);
        if (user == null) return null;

        var address = new Address
        {
            UserId = userId,
            Title = "آدرس جدید",
            Province = "نامشخص",
            City = city,
            Street = fullAddress,
            PostalCode = postalCode,
            ReceiverName = user.FullName,
            ReceiverPhone = phone
        };

        user.Addresses.Add(address);
        await _userRepository.UpdateAsync(user);

        return MapAddressToDto(address);
    }

    public async Task<AddressDto?> UpdateAddress(int addressId, string fullAddress, string city, string postalCode, string phone)
    {
        var address = await _userRepository.GetAddressByIdAsync(addressId);
        if (address == null) return null;

        address.Street = fullAddress;
        address.City = city;
        address.PostalCode = postalCode;
        address.ReceiverPhone = phone;

        await _userRepository.UpdateAddressAsync(address);

        return MapAddressToDto(address);
    }

    public async Task<bool> DeleteAddress(int addressId)
    {
        var address = await _userRepository.GetAddressByIdAsync(addressId);
        if (address == null) return false;

        await _userRepository.DeleteAddressAsync(addressId);
        return true;
    }

    private static AddressDto MapAddressToDto(Address a)
    {
        return new AddressDto
        {
            Id = a.Id,
            UserId = a.UserId,
            Title = a.Title,
            Province = a.Province,
            City = a.City,
            Street = a.Street,
            PostalCode = a.PostalCode,
            Latitude = a.Latitude,
            Longitude = a.Longitude,
            ReceiverName = a.ReceiverName,
            ReceiverPhone = a.ReceiverPhone
        };
    }
}
