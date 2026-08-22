using Microsoft.EntityFrameworkCore;
using tagr.Data;
using tagr.Models;
using tagr.Repositories.Interfaces;

namespace tagr.Repositories.Implementations
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly ApplicationDbContext _context;

        public WishlistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Wishlist>> GetByCustomerIdAsync(string customerId)
            => await _context.Wishlists
                .Include(w => w.Product)
                .Where(w => w.CustomerId == customerId)
                .OrderByDescending(w => w.Id)
                .ToListAsync();

        public async Task<Wishlist?> GetItemAsync(string customerId, int productId)
            => await _context.Wishlists
                .FirstOrDefaultAsync(w => w.CustomerId == customerId && w.ProductId == productId);

        public async Task<bool> ExistsAsync(string customerId, int productId)
            => await _context.Wishlists
                .AnyAsync(w => w.CustomerId == customerId && w.ProductId == productId);

        public async Task AddAsync(Wishlist item)
            => await _context.Wishlists.AddAsync(item);

        public void Remove(Wishlist item)
            => _context.Wishlists.Remove(item);
    }
}
