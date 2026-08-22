using tagr.ViewModels;

namespace tagr.Services.Interfaces
{
    public interface IWishlistService
    {
        Task<List<WishlistItemViewModel>> GetAsync();
        Task AddAsync(int productId);
        Task RemoveAsync(int productId);
    }
}
