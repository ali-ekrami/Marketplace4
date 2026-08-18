//using tagr.Models;
//using tagr.ViewModels;

//namespace tagr.Mapping
//{
//    public static class OrderMappingExtensions
//    {
//        public static OrderListItemViewModel ToListItemViewModel(this Order order) =>
//            new()
//            {
//                Id = order.Id,
//                OrderDate = order.OrderDate,
//                TotalAmount = order.TotalAmount,
//                Status = order.Status,
//                ItemsCount = order.OrderItems.Count
//            };

//        public static List<OrderListItemViewModel> ToListItemViewModels(this IEnumerable<Order> orders) =>
//            orders.Select(o => o.ToListItemViewModel()).ToList();

//        public static OrderItemDetailsViewModel ToDetailsViewModel(this OrderItem item) =>
//            new()
//            {
//                ProductName = item.Product?.Name ?? string.Empty,
//                Quantity = item.Quantity,
//                UnitPrice = item.UnitPrice
//            };

//        public static OrderDetailsViewModel ToDetailsViewModel(this Order order) =>
//            new()
//            {
//                Id = order.Id,
//                CustomerName = order.Customer?.FullName ?? string.Empty,
//                OrderDate = order.OrderDate,
//                TotalAmount = order.TotalAmount,
//                Status = order.Status,
//                Items = order.OrderItems.Select(i => i.ToDetailsViewModel()).ToList()
//            };
//    }
//}