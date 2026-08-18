//using tagr.Models;
//using tagr.ViewModels;

//namespace tagr.Mapping
//{
//    public static class ReviewMappingExtensions
//    {
//        public static ReviewListItemViewModel ToListItemViewModel(this Review review) =>
//            new()
//            {
//                Id = review.Id,
//                CustomerName = review.Customer?.FullName ?? string.Empty,
//                Rating = review.Rating,
//                Comment = review.Comment,
//                CreatedAt = review.CreatedAt
//            };

//        public static List<ReviewListItemViewModel> ToListItemViewModels(this IEnumerable<Review> reviews) =>
//            reviews.Select(r => r.ToListItemViewModel()).ToList();

//        public static Review ToEntity(this ReviewCreateViewModel model, string customerId) =>
//            new()
//            {
//                ProductId = model.ProductId,
//                Rating = model.Rating,
//                Comment = model.Comment,
//                CustomerId = customerId
//            };
//    }

//    public static class WishlistMappingExtensions
//    {
//        public static WishlistItemViewModel ToItemViewModel(this Wishlist wishlist) =>
//            new()
//            {
//                Id = wishlist.Id,
//                ProductId = wishlist.ProductId,
//                ProductName = wishlist.Product?.Name ?? string.Empty,
//                ProductPrice = wishlist.Product?.Price ?? 0,
//                ProductImageUrl = wishlist.Product?.ImageUrl ?? string.Empty
//            };

//        public static List<WishlistItemViewModel> ToItemViewModels(this IEnumerable<Wishlist> wishlists) =>
//            wishlists.Select(w => w.ToItemViewModel()).ToList();
//    }
//}