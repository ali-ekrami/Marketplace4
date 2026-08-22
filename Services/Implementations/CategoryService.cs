using tagr.Exceptions;
using tagr.Mapping;
using tagr.Models;
using tagr.Services.Interfaces;
using tagr.UnitOfWork;
using tagr.ViewModels;

namespace tagr.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CategoryListItemViewModel>> GetAllAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllWithProductsAsync();
            return categories.ToListItemViewModels();
        }

        public async Task<CategoryEditViewModel> GetForEditAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Category), id);
            return category.ToEditViewModel();
        }

        public async Task<CategoryDeleteViewModel> GetForDeleteAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdWithProductsAsync(id) ?? throw new NotFoundException(nameof(Category), id);
            return category.ToDeleteViewModel();
        }

        public async Task CreateAsync(CategoryCreateViewModel model)
        {
            model.Name = model.Name.Trim();

            var exists = await _unitOfWork.Categories.ExistsByNameAsync(model.Name);
            if (exists) throw new DuplicateEntityException(nameof(model.Name), "A category with this name already exists.");

            var category = model.ToEntity();

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, CategoryEditViewModel model)
        {
            model.Name = model.Name.Trim();

            var exists = await _unitOfWork.Categories.ExistsByNameAsync(model.Name, id);
            if (exists) throw new DuplicateEntityException(nameof(model.Name), "A category with this name already exists.");

            var category = await _unitOfWork.Categories.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Category), id);

            model.ApplyTo(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdWithProductsAsync(id) ?? throw new NotFoundException(nameof(Category), id);

            if (category.Products.Any()) throw new BusinessRuleException("This category cannot be deleted because it contains products.");

            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}