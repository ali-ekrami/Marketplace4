using tagr.Models;

namespace tagr.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllWithDetailsAsync();
        Task<List<Order>> GetByCustomerIdAsync(string customerId);
        Task<List<Order>> GetBySellerIdAsync(string sellerId);
        Task<Order?> GetByIdAsync(int id);
        Task<Order?> GetByIdWithDetailsAsync(int id);
        Task<Order?> GetByIdWithItemsAsync(int id);
        Task<bool> ContainsSellerProductsAsync(int orderId, string sellerId);
        Task<bool> HasPurchasedProductAsync(string customerId, int productId);
        Task AddAsync(Order order);
    }
}
