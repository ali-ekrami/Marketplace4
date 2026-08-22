using Microsoft.EntityFrameworkCore;
using tagr.Data;
using tagr.Models;
using tagr.Repositories.Interfaces;

namespace tagr.Repositories.Implementations
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Review>> GetByProductIdAsync(int productId)
            => await _context.Reviews
                .AsNoTracking()
                .Include(r => r.Customer)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<Review?> GetByIdAsync(int id)
            => await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<bool> ExistsForCustomerAsync(string customerId, int productId)
            => await _context.Reviews
                .AnyAsync(r => r.CustomerId == customerId && r.ProductId == productId);

        public async Task AddAsync(Review review)
            => await _context.Reviews.AddAsync(review);

        public void Remove(Review review)
            => _context.Reviews.Remove(review);
    }
}
