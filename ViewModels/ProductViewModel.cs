using System.ComponentModel.DataAnnotations;

namespace tagr.ViewModels
{
    public class ProductCreateViewModel
    {
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(150, ErrorMessage = "Product name cannot exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
        public int StockQuantity { get; set; }

        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters.")]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }

        public List<CategoryOptionViewModel> AvailableCategories { get; set; } = new();
    }

    public class ProductEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(150, ErrorMessage = "Product name cannot exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
        public int StockQuantity { get; set; }

        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters.")]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }

        public List<CategoryOptionViewModel> AvailableCategories { get; set; } = new();
    }

    public class ProductListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
    }

    public class ProductDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int ReviewsCount { get; set; }

        public List<ReviewListItemViewModel> Reviews { get; set; } = new();

        // Filled in only when the page is requested by a signed-in customer.
        public bool IsInWishlist { get; set; }
        public bool HasReviewed { get; set; }
        public bool CanReview { get; set; }
    }
}