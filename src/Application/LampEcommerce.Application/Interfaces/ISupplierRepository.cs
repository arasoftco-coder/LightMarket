using System.Collections.Generic;
using System.Threading.Tasks;
using LampEcommerce.Domain.Entities;

namespace LampEcommerce.Application.Interfaces;

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(int id);
    Task<IEnumerable<Supplier>> GetAllAsync();
    Task<Supplier> AddAsync(Supplier supplier);
    Task UpdateAsync(Supplier supplier);
}
