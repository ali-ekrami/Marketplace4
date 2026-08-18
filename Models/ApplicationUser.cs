using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace tagr.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(150, ErrorMessage = "Full name cannot exceed 150 characters.")]
        public string FullName { get; set; } = string.Empty;

        public bool IsSellerApproved { get; set; } = false;

        public bool IsSuspended { get; set; } = false;

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    }
}
