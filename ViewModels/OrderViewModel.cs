using System.ComponentModel.DataAnnotations;
using tagr.Models;

namespace tagr.ViewModels
{
    public class OrderCreateViewModel
    {
        [Required(ErrorMessage = "Please add at least one item to the order.")]
        [MinLength(1, ErrorMessage = "Please add at least one item to the order.")]
        public List<OrderItemCreateViewModel> Items { get; set; } = new();

        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Shipping address is required.")]
        [StringLength(300, ErrorMessage = "Shipping address cannot exceed 300 characters.")]
        public string ShippingAddress { get; set; } = string.Empty;
    }

    public class CheckoutViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
        public string City { get; set; } = string.Empty;

        // For redisplaying the order summary; not posted back by the form.
        public List<CartItemViewModel> Items { get; set; } = new();
        public decimal Total { get; set; }
    }

    public class OrderItemCreateViewModel
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }

    public class OrderListItemViewModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int ItemsCount { get; set; }
        public bool CanCancel => Status.CanBeCancelled();
    }

    public class OrderDetailsViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public List<OrderItemDetailsViewModel> Items { get; set; } = new();
        public bool CanCancel => Status.CanBeCancelled();
    }

    public class OrderItemDetailsViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => Quantity * UnitPrice;
    }

    public class OrderStatusUpdateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a status.")]
        public OrderStatus Status { get; set; }
    }

    // An order as one seller sees it: only their own lines, never the whole basket.
    public class SellerOrderListItemViewModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public int MyItemsCount { get; set; }
        public decimal MyItemsTotal { get; set; }
    }

    public class SellerOrderDetailsViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;

        // Only the lines belonging to the current seller.
        public List<OrderItemDetailsViewModel> Items { get; set; } = new();
        public decimal MyItemsTotal => Items.Sum(i => i.LineTotal);
    }
}
