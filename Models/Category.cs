using System.ComponentModel.DataAnnotations;

namespace tagr.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
