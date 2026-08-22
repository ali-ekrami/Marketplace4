using tagr.ViewModels;

namespace tagr.Services.Interfaces
{
    public interface IReviewService
    {
        Task<List<ReviewListItemViewModel>> GetByProductIdAsync(int productId);
        Task CreateAsync(ReviewCreateViewModel model, string customerId);
    }
}
