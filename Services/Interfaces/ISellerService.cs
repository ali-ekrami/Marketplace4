using tagr.Models;

namespace tagr.Services.Interfaces
{
    public interface ISellerService
    {
        Task<List<ApplicationUser>> GetPendingRequestsAsync();
        Task ApproveAsync(string userId);
        Task RejectAsync(string userId);
        Task RequestAsync(string userId);
    }
}
