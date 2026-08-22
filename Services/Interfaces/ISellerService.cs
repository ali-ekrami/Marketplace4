using tagr.ViewModels;

namespace tagr.Services.Interfaces
{
    public interface ISellerService
    {
        Task<List<SellerRequestListItemViewModel>> GetPendingRequestsAsync();
        Task<SellerStatusViewModel> GetStatusAsync(string userId);
        Task ApproveAsync(string userId);
        Task RejectAsync(string userId);
        Task RequestAsync(string userId);
    }
}
