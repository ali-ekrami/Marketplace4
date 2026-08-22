using tagr.Models;
using tagr.ViewModels;

namespace tagr.Mapping
{
    public static class ApplicationUserMappingExtensions
    {
        public static UserProfileViewModel ToProfileViewModel(this ApplicationUser user) =>
            new()
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                IsSellerApproved = user.IsSellerApproved,
                IsSuspended = user.IsSuspended,
                ProductsCount = user.Products.Count,
                OrdersCount = user.Orders.Count
            };

        public static UserEditViewModel ToEditViewModel(this ApplicationUser user) =>
            new()
            {
                Id = user.Id,
                FullName = user.FullName
            };

        public static void ApplyTo(this UserEditViewModel model, ApplicationUser user)
        {
            user.FullName = model.FullName;
        }

        public static UserAdminListItemViewModel ToAdminListItemViewModel(this ApplicationUser user) =>
            new()
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                IsSellerApproved = user.IsSellerApproved,
                IsSuspended = user.IsSuspended
            };

        public static List<UserAdminListItemViewModel> ToAdminListItemViewModels(this IEnumerable<ApplicationUser> users) =>
            users.Select(u => u.ToAdminListItemViewModel()).ToList();

        public static SellerRequestListItemViewModel ToSellerRequestViewModel(this ApplicationUser user) =>
            new()
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty
            };

        public static List<SellerRequestListItemViewModel> ToSellerRequestViewModels(this IEnumerable<ApplicationUser> users) =>
            users.Select(u => u.ToSellerRequestViewModel()).ToList();

        public static SellerStatusViewModel ToSellerStatusViewModel(this ApplicationUser user) =>
            new()
            {
                FullName = user.FullName,
                IsSellerApproved = user.IsSellerApproved,
                IsSellerRequested = user.IsSellerRequested
            };
    }
}
