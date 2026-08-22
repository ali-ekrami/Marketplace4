using tagr.Models;
using tagr.ViewModels;

namespace tagr.Mapping
{
    public static class ProductMappingExtensions
    {
        public static ProductListItemViewModel ToListItemViewModel(this Product product) =>
            new()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                CategoryName = product.Category?.Name ?? string.Empty,
                SellerName = product.Seller?.FullName ?? string.Empty
            };

        public static List<ProductListItemViewModel> ToListItemViewModels(this IEnumerable<Product> products) =>
            products.Select(p => p.ToListItemViewModel()).ToList();

        public static ProductDetailsViewModel ToDetailsViewModel(this Product product) =>
            new()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                CategoryName = product.Category?.Name ?? string.Empty,
                SellerName = product.Seller?.FullName ?? string.Empty,
                ReviewsCount = product.Reviews.Count,
                AverageRating = product.Reviews.Any() ? product.Reviews.Average(r => r.Rating) : 0,
                Reviews = product.Reviews.OrderByDescending(r => r.CreatedAt).ToListItemViewModels()
            };

        public static ProductEditViewModel ToEditViewModel(this Product product) =>
            new()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId
            };

        public static Product ToEntity(this ProductCreateViewModel model, string sellerId) =>
            new()
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                ImageUrl = model.ImageUrl,
                CategoryId = model.CategoryId,
                SellerId = sellerId
            };

        public static void ApplyTo(this ProductEditViewModel model, Product product)
        {
            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.StockQuantity = model.StockQuantity;
            product.ImageUrl = model.ImageUrl;
            product.CategoryId = model.CategoryId;
        }
    }
}