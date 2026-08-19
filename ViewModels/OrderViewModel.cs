using System.ComponentModel.DataAnnotations;
using tagr.Models;

namespace tagr.ViewModels
{
    public class OrderCreateViewModel
    {
        [Required(ErrorMessage = "Please add at least one item to the order.")]
        [MinLength(1, ErrorMessage = "Please add at least one item to the order.")]
        public List<OrderItemCreateViewModel> Items { get; set; } = new();
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
        public int ItemsCount { get; set; }
    }

    public class OrderDetailsViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItemDetailsViewModel> Items { get; set; } = new();
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
}