using tagr.ViewModels;

namespace tagr.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderListItemViewModel>> GetAllAsync();
        Task<List<OrderListItemViewModel>> GetByCustomerIdAsync(string customerId);

        Task<OrderDetailsViewModel> GetDetailsAsync(int id);

        Task<string> GetOwnerIdAsync(int id);

        Task<int> CreateAsync(OrderCreateViewModel model, string customerId);

        Task UpdateStatusAsync(OrderStatusUpdateViewModel model);

        Task CancelAsync(int id);

        // Seller-scoped: an order is only visible to a seller who owns one of its lines.
        Task<List<SellerOrderListItemViewModel>> GetBySellerIdAsync(string sellerId);
        Task<SellerOrderDetailsViewModel> GetDetailsForSellerAsync(int id, string sellerId);
        Task UpdateStatusBySellerAsync(OrderStatusUpdateViewModel model, string sellerId);
    }
}
