using System.ComponentModel.DataAnnotations;

namespace tagr.ViewModels
{
    public class CategoryCreateViewModel
    {
        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;
    }

    public class CategoryEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;
    }

    public class CategoryListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProductsCount { get; set; }
    }

    public class CategoryDeleteViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProductsCount { get; set; }
    }

    public class CategoryOptionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
