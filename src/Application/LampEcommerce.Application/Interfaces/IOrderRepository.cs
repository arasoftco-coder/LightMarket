using System.Collections.Generic;
using System.Threading.Tasks;
using LampEcommerce.Domain.Entities;

namespace LampEcommerce.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task<Order?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Order>> GetAllAsync();
    Task<IEnumerable<Order>> GetAllWithDetailsAsync();
    Task<IEnumerable<Order>> GetByUserIdAsync(int userId);
    Task<Order> AddAsync(Order order);
    Task UpdateAsync(Order order);
    Task DeleteOrderItemAsync(int orderItemId);
}
