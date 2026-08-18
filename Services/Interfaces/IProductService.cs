using tagr.ViewModels;

namespace tagr.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductListItemViewModel>> GetAllAsync();
        Task<List<ProductListItemViewModel>> SearchAsync(string? searchString, int? categoryId, string? sortOrder);
        Task<List<ProductListItemViewModel>> GetBySellerIdAsync(string sellerId);
        Task<List<ProductListItemViewModel>> GetByCategoryIdAsync(int categoryId);

        Task<ProductDetailsViewModel> GetDetailsAsync(int id);

        Task<ProductCreateViewModel> GetCreateFormAsync();
        Task<ProductEditViewModel> GetForEditAsync(int id);
        Task<List<CategoryOptionViewModel>> GetAvailableCategoriesAsync();

        Task<string> GetOwnerIdAsync(int id);

        Task CreateAsync(ProductCreateViewModel model, string sellerId);
        Task UpdateAsync(int id, ProductEditViewModel model);
        Task UpdateStockAsync(int id, int stockQuantity);
        Task DeleteAsync(int id);
    }
}