using tagr.Models;
using tagr.ViewModels;

namespace tagr.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryListItemViewModel>> GetAllAsync();
        Task<CategoryEditViewModel> GetForEditAsync(int id);
        Task<CategoryDeleteViewModel> GetForDeleteAsync(int id);
        Task CreateAsync(CategoryCreateViewModel model);
        Task UpdateAsync(int id, CategoryEditViewModel model);
        Task DeleteAsync(int id);
    }
}
