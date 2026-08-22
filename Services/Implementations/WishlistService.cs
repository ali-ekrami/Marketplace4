using System.Security.Claims;
using tagr.Exceptions;
using tagr.Mapping;
using tagr.Models;
using tagr.Services.Interfaces;
using tagr.UnitOfWork;
using tagr.ViewModels;

namespace tagr.Services.Implementations
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WishlistService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        private string CustomerId =>
            _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new InvalidOperationException("No authenticated user is available.");

        public async Task<List<WishlistItemViewModel>> GetAsync()
        {
            var items = await _unitOfWork.Wishlists.GetByCustomerIdAsync(CustomerId);

            // The product may have been deleted since it was saved.
            var orphans = items.Where(i => i.Product == null).ToList();

            if (orphans.Count > 0)
            {
                foreach (var orphan in orphans)
                {
                    _unitOfWork.Wishlists.Remove(orphan);
                    items.Remove(orphan);
                }

                await _unitOfWork.SaveChangesAsync();
            }

            return items.ToItemViewModels();
        }

        public async Task AddAsync(int productId)
        {
            _ = await _unitOfWork.Products.GetByIdAsync(productId)
                ?? throw new NotFoundException(nameof(Product), productId);

            var customerId = CustomerId;

            if (await _unitOfWork.Wishlists.ExistsAsync(customerId, productId))
            {
                throw new BusinessRuleException("This product is already in your wishlist.");
            }

            await _unitOfWork.Wishlists.AddAsync(new Wishlist
            {
                CustomerId = customerId,
                ProductId = productId
            });

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveAsync(int productId)
        {
            var existing = await _unitOfWork.Wishlists.GetItemAsync(CustomerId, productId);

            if (existing != null)
            {
                _unitOfWork.Wishlists.Remove(existing);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
