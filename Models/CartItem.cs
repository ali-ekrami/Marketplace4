using System.ComponentModel.DataAnnotations;

namespace tagr.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        public string CustomerId { get; set; } = string.Empty;
        public virtual ApplicationUser Customer { get; set; } = null!;

        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
