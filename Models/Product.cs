using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tagr.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(150, ErrorMessage = "Product name cannot exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
        public int StockQuantity { get; set; }

        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters.")]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        [Required]
        public string SellerId { get; set; } = string.Empty;
        public virtual ApplicationUser Seller { get; set; } = null!;

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
