using tagr.Models;

namespace tagr.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<List<CartItem>> GetByCustomerIdAsync(string customerId);
        Task<CartItem?> GetItemAsync(string customerId, int productId);
        Task AddAsync(CartItem item);
        void Update(CartItem item);
        void Remove(CartItem item);
        Task RemoveRangeByCustomerIdAsync(string customerId);
    }
}
