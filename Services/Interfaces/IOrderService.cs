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
    }
}
