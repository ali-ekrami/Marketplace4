using tagr.Exceptions;
using tagr.Mapping;
using tagr.Models;
using tagr.Services.Interfaces;
using tagr.UnitOfWork;
using tagr.ViewModels;

namespace tagr.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ProductListItemViewModel>> GetAllAsync()
        {
            var products = await _unitOfWork.Products.GetAllWithDetailsAsync();
            return products.ToListItemViewModels();
        }

        public async Task<List<ProductListItemViewModel>> SearchAsync(string? searchString, int? categoryId, string? sortOrder)
        {
            var products = await _unitOfWork.Products.SearchAsync(searchString, categoryId, sortOrder);
            return products.ToListItemViewModels();
        }

        public async Task<List<ProductListItemViewModel>> GetBySellerIdAsync(string sellerId)
        {
            var products = await _unitOfWork.Products.GetBySellerIdAsync(sellerId);
            return products.ToListItemViewModels();
        }

        public async Task<List<ProductListItemViewModel>> GetByCategoryIdAsync(int categoryId)
        {
            var products = await _unitOfWork.Products.GetByCategoryIdAsync(categoryId);
            return products.ToListItemViewModels();
        }

        public async Task<ProductDetailsViewModel> GetDetailsAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdWithDetailsAsync(id)
                ?? throw new NotFoundException(nameof(Product), id);

            return product.ToDetailsViewModel();
        }

        public async Task<List<CategoryOptionViewModel>> GetAvailableCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllWithProductsAsync();
            return categories.ToOptionViewModels();
        }

        public async Task<ProductCreateViewModel> GetCreateFormAsync()
        {
            return new ProductCreateViewModel
            {
                AvailableCategories = await GetAvailableCategoriesAsync()
            };
        }

        public async Task<ProductEditViewModel> GetForEditAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(Product), id);

            var model = product.ToEditViewModel();

            var categories = await _unitOfWork.Categories.GetAllWithProductsAsync();
            model.AvailableCategories = categories.ToOptionViewModels();

            return model;
        }

        public async Task<string> GetOwnerIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(Product), id);

            return product.SellerId;
        }

        public async Task CreateAsync(ProductCreateViewModel model, string sellerId)
        {
            model.Name = model.Name.Trim();

            var categoryExists = await _unitOfWork.Categories.GetByIdAsync(model.CategoryId);
            if (categoryExists == null)
            {
                throw new NotFoundException(nameof(Category), model.CategoryId);
            }

            if (await _unitOfWork.Products.ExistsByNameForSellerAsync(model.Name, sellerId))
            {
                throw new DuplicateEntityException(
                    nameof(model.Name),
                    "You already have a product with this name.");
            }

            var product = model.ToEntity(sellerId);

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, ProductEditViewModel model)
        {
            model.Name = model.Name.Trim();

            var existing = await _unitOfWork.Products.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(Product), id);

            var categoryExists = await _unitOfWork.Categories.GetByIdAsync(model.CategoryId);
            if (categoryExists == null)
            {
                throw new NotFoundException(nameof(Category), model.CategoryId);
            }

            if (await _unitOfWork.Products.ExistsByNameForSellerAsync(model.Name, existing.SellerId, id))
            {
                throw new DuplicateEntityException(
                    nameof(model.Name),
                    "You already have a product with this name.");
            }

            model.ApplyTo(existing);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateStockAsync(int id, int stockQuantity)
        {
            if (stockQuantity < 0)
            {
                throw new BusinessRuleException("Stock quantity cannot be negative.");
            }

            var product = await _unitOfWork.Products.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(Product), id);

            product.StockQuantity = stockQuantity;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdWithDetailsAsync(id)
                ?? throw new NotFoundException(nameof(Product), id);

            _unitOfWork.Products.Remove(product);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}