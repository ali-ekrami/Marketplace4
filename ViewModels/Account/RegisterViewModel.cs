using System.ComponentModel.DataAnnotations;

namespace tagr.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "full name is required")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "email is required")]
        [EmailAddress(ErrorMessage = "invalid email format")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "password is required")]
        [StringLength(100, ErrorMessage = "password must be at least {2} characters long", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "Register as a seller?")]
        public bool IsSeller { get; set; } = false;
    }
}
