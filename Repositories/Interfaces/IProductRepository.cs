using tagr.Models;

namespace tagr.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllWithDetailsAsync();

        Task<List<Product>> SearchAsync(string? searchString, int? categoryId, string? sortOrder);

        Task<Product?> GetByIdAsync(int id);

        Task<Product?> GetByIdWithDetailsAsync(int id);

        Task<List<Product>> GetBySellerIdAsync(string sellerId);

        Task<List<Product>> GetByCategoryIdAsync(int categoryId);

        Task<bool> ExistsByNameForSellerAsync(string name, string sellerId, int? excludeId = null);

        Task AddAsync(Product product);

        void Update(Product product);

        void Remove(Product product);
    }
}