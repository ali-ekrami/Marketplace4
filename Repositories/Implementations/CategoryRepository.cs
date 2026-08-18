using Microsoft.EntityFrameworkCore;
using tagr.Data;
using tagr.Models;
using tagr.Repositories.Interfaces;

namespace tagr.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context; 
        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllWithProductsAsync()
            => await _context.Categories
            .AsNoTracking()
            .Include(c => c.Products)
            .OrderBy(c => c.Name)
            .ToListAsync();

        public async Task<Category?> GetByIdAsync(int id)
            => await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Category?> GetByIdWithProductsAsync(int id)
            => await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
            => await _context.Categories
            .AnyAsync(c => c.Name.ToLower() == name.ToLower() && (!excludeId.HasValue || c.Id != excludeId.Value));

        public async Task AddAsync(Category category)
            => await _context.Categories.AddAsync(category);

        public void Update(Category category)
            => _context.Categories.Update(category);

        public void Delete(Category category)
            => _context.Categories.Remove(category);
    }
}
