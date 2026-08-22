using tagr.Models;
using tagr.Data;
using tagr.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace tagr.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllWithDetailsAsync()
            => await _context.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        public async Task<List<Order>> GetByCustomerIdAsync(string customerId)
            => await _context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        // Orders that contain at least one product owned by this seller.
        public async Task<List<Order>> GetBySellerIdAsync(string sellerId)
            => await _context.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Where(o => o.OrderItems.Any(oi => oi.Product.SellerId == sellerId))
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        public async Task<Order?> GetByIdAsync(int id)
            => await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<Order?> GetByIdWithDetailsAsync(int id)
            => await _context.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        // Tracked (no AsNoTracking) because cancelling writes back to the order and its products.
        public async Task<Order?> GetByIdWithItemsAsync(int id)
            => await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<bool> ContainsSellerProductsAsync(int orderId, string sellerId)
            => await _context.OrderItems
            .AnyAsync(oi => oi.OrderId == orderId && oi.Product.SellerId == sellerId);

        // A customer may only review a product they actually bought (cancelled orders do not count).
        public async Task<bool> HasPurchasedProductAsync(string customerId, int productId)
            => await _context.Orders
            .AnyAsync(o => o.CustomerId == customerId
                        && o.Status != OrderStatus.Cancelled
                        && o.OrderItems.Any(oi => oi.ProductId == productId));

        public async Task AddAsync(Order order)
            =>await _context.Orders.AddAsync(order);
    }
}
