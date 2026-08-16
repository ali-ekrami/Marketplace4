using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace tagr.Models
{
    public class ApplicationUser : IdentityUser
    {

        public string FullName { get; set; } = string.Empty;
        public bool IsSellerApproved { get; set; } = false; // للبائعين فقط
        public bool IsSuspended { get; set; } = false; // لتجميد الحساب من قبل الأدمن

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    }
}
