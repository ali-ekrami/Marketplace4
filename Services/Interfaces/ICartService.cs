using tagr.ViewModels;

namespace tagr.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartViewModel> GetCartAsync();
        Task<int> GetItemCountAsync();

        Task AddToCartAsync(int productId, int quantity);
        Task UpdateQuantityAsync(int productId, int quantity);
        Task RemoveFromCartAsync(int productId);
        Task ClearCartAsync();
    }
}
