using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace tagr.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        // العلاقات (Category & Seller)
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        public string SellerId { get; set; } = string.Empty;
        public virtual ApplicationUser Seller { get; set; } = null!;

        public virtual ICollection<Order> OrderItems { get; set; } = new List<Order>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
