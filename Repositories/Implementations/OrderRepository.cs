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

        public async Task AddAsync(Order order)
            =>await _context.Orders.AddAsync(order);
    }
}
