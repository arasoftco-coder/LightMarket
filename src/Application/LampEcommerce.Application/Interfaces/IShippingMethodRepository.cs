using System.Collections.Generic;
using System.Threading.Tasks;
using LampEcommerce.Domain.Entities;

namespace LampEcommerce.Application.Interfaces;

public interface IShippingMethodRepository
{
    Task<ShippingMethod?> GetByIdAsync(int id);
    Task<IEnumerable<ShippingMethod>> GetAllAsync();
    Task<ShippingMethod> AddAsync(ShippingMethod method);
    Task UpdateAsync(ShippingMethod method);
    Task DeleteAsync(int id);
}
