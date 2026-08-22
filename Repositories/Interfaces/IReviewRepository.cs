using tagr.Models;

namespace tagr.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetByProductIdAsync(int productId);
        Task<Review?> GetByIdAsync(int id);
        Task<bool> ExistsForCustomerAsync(string customerId, int productId);
        Task AddAsync(Review review);
        void Remove(Review review);
    }
}
