using tagr.Models;

namespace tagr.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllWithDetailsAsync();
        Task<List<Order>> GetByCustomerIdAsync(string customerId);
        Task<Order?> GetByIdAsync(int id);
        Task<Order?> GetByIdWithDetailsAsync(int id);
        Task AddAsync(Order order);
    }
}