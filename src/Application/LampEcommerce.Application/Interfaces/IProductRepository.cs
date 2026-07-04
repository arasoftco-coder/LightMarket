using System.Collections.Generic;
using System.Threading.Tasks;
using LampEcommerce.Domain.Entities;

namespace LampEcommerce.Application.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product> AddAsync(Product product);
    Task UpdateAsync(Product product);
}
