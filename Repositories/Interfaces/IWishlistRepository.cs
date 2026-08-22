using tagr.Models;

namespace tagr.Repositories.Interfaces
{
    public interface IWishlistRepository
    {
        Task<List<Wishlist>> GetByCustomerIdAsync(string customerId);
        Task<Wishlist?> GetItemAsync(string customerId, int productId);
        Task<bool> ExistsAsync(string customerId, int productId);
        Task AddAsync(Wishlist item);
        void Remove(Wishlist item);
    }
}
