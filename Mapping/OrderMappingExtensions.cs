using tagr.Models;
using tagr.ViewModels;

namespace tagr.Mapping
{
    public static class OrderMappingExtensions
    {
        public static OrderListItemViewModel ToListItemViewModel(this Order order) =>
            new()
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CustomerName = order.Customer?.FullName ?? string.Empty,
                ItemsCount = order.OrderItems.Count
            };

        public static List<OrderListItemViewModel> ToListItemViewModels(this IEnumerable<Order> orders) =>
            orders.Select(o => o.ToListItemViewModel()).ToList();

        public static OrderItemDetailsViewModel ToDetailsViewModel(this OrderItem item) =>
            new()
            {
                ProductName = item.Product?.Name ?? string.Empty,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            };

        public static OrderDetailsViewModel ToDetailsViewModel(this Order order) =>
            new()
            {
                Id = order.Id,
                CustomerName = order.Customer?.FullName ?? string.Empty,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                PhoneNumber = order.PhoneNumber,
                ShippingAddress = order.ShippingAddress,
                Items = order.OrderItems.Select(i => i.ToDetailsViewModel()).ToList()
            };

        // ===== Seller-scoped: only the lines whose product belongs to this seller =====

        private static List<OrderItem> ItemsOfSeller(Order order, string sellerId) =>
            order.OrderItems.Where(i => i.Product != null && i.Product.SellerId == sellerId).ToList();

        public static SellerOrderListItemViewModel ToSellerListItemViewModel(this Order order, string sellerId)
        {
            var myItems = ItemsOfSeller(order, sellerId);

            return new()
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                CustomerName = order.Customer?.FullName ?? string.Empty,
                Status = order.Status,
                MyItemsCount = myItems.Count,
                MyItemsTotal = myItems.Sum(i => i.Quantity * i.UnitPrice)
            };
        }

        public static List<SellerOrderListItemViewModel> ToSellerListItemViewModels(this IEnumerable<Order> orders, string sellerId) =>
            orders.Select(o => o.ToSellerListItemViewModel(sellerId)).ToList();

        public static SellerOrderDetailsViewModel ToSellerDetailsViewModel(this Order order, string sellerId) =>
            new()
            {
                Id = order.Id,
                CustomerName = order.Customer?.FullName ?? string.Empty,
                OrderDate = order.OrderDate,
                Status = order.Status,
                PhoneNumber = order.PhoneNumber,
                ShippingAddress = order.ShippingAddress,
                Items = ItemsOfSeller(order, sellerId).Select(i => i.ToDetailsViewModel()).ToList()
            };
    }
}
