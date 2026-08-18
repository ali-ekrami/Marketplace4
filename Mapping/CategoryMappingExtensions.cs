using tagr.Models;
using tagr.ViewModels;

namespace tagr.Mapping
{
    public static class CategoryMappingExtensions
    {
        public static CategoryListItemViewModel ToListItemViewModel(this Category category) =>
            new()
            {
                Id = category.Id,
                Name = category.Name,
                ProductsCount = category.Products.Count
            };

        public static List<CategoryListItemViewModel> ToListItemViewModels(this IEnumerable<Category> categories) =>
            categories.Select(c => c.ToListItemViewModel()).ToList();

        public static CategoryEditViewModel ToEditViewModel(this Category category) =>
            new()
            {
                Id = category.Id,
                Name = category.Name
            };

        public static CategoryDeleteViewModel ToDeleteViewModel(this Category category) =>
            new()
            {
                Id = category.Id,
                Name = category.Name,
                ProductsCount = category.Products.Count
            };

        public static CategoryOptionViewModel ToOptionViewModel(this Category category) =>
            new()
            {
                Id = category.Id,
                Name = category.Name
            };

        public static List<CategoryOptionViewModel> ToOptionViewModels(this IEnumerable<Category> categories) =>
            categories.Select(c => c.ToOptionViewModel()).ToList();

        public static Category ToEntity(this CategoryCreateViewModel model) =>
            new()
            {
                Name = model.Name
            };

        // بيحدّث الـ Entity الموجود بدل ما يعمل واحد جديد (أنضف مع الـ Change Tracker)
        public static void ApplyTo(this CategoryEditViewModel model, Category category)
        {
            category.Name = model.Name;
        }
    }
}