using System.ComponentModel.DataAnnotations;

namespace tagr.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string CustomerId { get; set; } = string.Empty;
        public virtual ApplicationUser Customer { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
    }

    public class Wishlist
    {
        public int Id { get; set; }

        [Required]
        public string CustomerId { get; set; } = string.Empty;
        public virtual ApplicationUser Customer { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
    }
}
