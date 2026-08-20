using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tagr.Models
{
    public enum OrderStatus { Pending, Confirmed, Shipped, Delivered, Cancelled }

    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string CustomerId { get; set; } = string.Empty;
        public virtual ApplicationUser Customer { get; set; } = null!;

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Range(0, double.MaxValue, ErrorMessage = "Total amount cannot be negative.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Required]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(300, ErrorMessage = "Shipping address cannot exceed 300 characters.")]
        public string ShippingAddress { get; set; } = string.Empty;

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;

        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
    }
}
