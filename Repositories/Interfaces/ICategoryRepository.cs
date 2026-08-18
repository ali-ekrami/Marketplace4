using tagr.Models;
using tagr.ViewModels;

namespace tagr.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllWithProductsAsync(); // Retrieve all categories with their associated products
        Task<Category?> GetByIdAsync(int id);
        Task<Category?> GetByIdWithProductsAsync(int id); // Retrieve a category by its ID along with its associated products
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task AddAsync(Category category);
        void Update(Category category);
        void Delete(Category category);
    }
}
