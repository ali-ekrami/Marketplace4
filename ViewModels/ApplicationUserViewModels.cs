using System.ComponentModel.DataAnnotations;

namespace tagr.ViewModels
{
    public class UserProfileViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsSellerApproved { get; set; }
        public bool IsSuspended { get; set; }
        public int ProductsCount { get; set; }
        public int OrdersCount { get; set; }
    }

    public class UserEditViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(150, ErrorMessage = "Full name cannot exceed 150 characters.")]
        public string FullName { get; set; } = string.Empty;
    }

    // للأدمن بس
    public class UserAdminListItemViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsSellerApproved { get; set; }
        public bool IsSuspended { get; set; }
    }
}