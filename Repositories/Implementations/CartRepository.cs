using tagr.Models;
using tagr.Data;
using tagr.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace tagr.Repositories.Implementations
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;
        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CartItem>> GetByCustomerIdAsync(string customerId)
            => await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.CustomerId == customerId)
            .OrderBy(c => c.AddedAt)
            .ToListAsync();

        public async Task<CartItem?> GetItemAsync(string customerId, int productId)
            => await _context.CartItems
            .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.ProductId == productId);

        public async Task AddAsync(CartItem item)
            => await _context.CartItems.AddAsync(item);

        public void Update(CartItem item)
            => _context.CartItems.Update(item);

        public void Remove(CartItem item)
            => _context.CartItems.Remove(item);

        public async Task RemoveRangeByCustomerIdAsync(string customerId)
        {
            var items = await _context.CartItems
                .Where(c => c.CustomerId == customerId)
                .ToListAsync();

            _context.CartItems.RemoveRange(items);
        }
    }
}
