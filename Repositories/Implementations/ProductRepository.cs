using Microsoft.EntityFrameworkCore;
using tagr.Data;
using tagr.Models;
using tagr.Repositories.Interfaces;

namespace tagr.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllWithDetailsAsync()
            => await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .OrderBy(p => p.Name)
                .ToListAsync();

        public async Task<List<Product>> SearchAsync(string? searchString, int? categoryId, string? sortOrder)
        {
            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(term) ||
                    p.Description.ToLower().Contains(term));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            query = sortOrder switch
            {
                "name_desc" => query.OrderByDescending(p => p.Name),
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                _ => query.OrderBy(p => p.Name)
            };

            return await query.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
            => await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Product?> GetByIdWithDetailsAsync(int id)
            => await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.Reviews)
                .ThenInclude(r => r.Customer)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<List<Product>> GetBySellerIdAsync(string sellerId)
            => await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Where(p => p.SellerId == sellerId)
                .OrderBy(p => p.Name)
                .ToListAsync();

        public async Task<List<Product>> GetByCategoryIdAsync(int categoryId)
            => await _context.Products
                .AsNoTracking()
                .Include(p => p.Seller)
                .Where(p => p.CategoryId == categoryId)
                .OrderBy(p => p.Name)
                .ToListAsync();

        public async Task<bool> ExistsByNameForSellerAsync(string name, string sellerId, int? excludeId = null)
            => await _context.Products
                .AnyAsync(p =>
                    p.Name.ToLower() == name.ToLower() &&
                    p.SellerId == sellerId &&
                    (!excludeId.HasValue || p.Id != excludeId.Value));

        public async Task AddAsync(Product product)
            => await _context.Products.AddAsync(product);

        public void Update(Product product)
            => _context.Products.Update(product);

        public void Remove(Product product)
            => _context.Products.Remove(product);
    }
}