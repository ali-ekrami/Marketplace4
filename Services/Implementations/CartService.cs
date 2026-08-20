using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using tagr.Exceptions;
using tagr.Models;
using tagr.Services.Interfaces;
using tagr.UnitOfWork;
using tagr.ViewModels;

namespace tagr.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        private string CustomerId =>
            _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new InvalidOperationException("No authenticated user is available.");

        public async Task<CartViewModel> GetCartAsync()
        {
            var customerId = CustomerId;
            var cartItems = await _unitOfWork.Carts.GetByCustomerIdAsync(customerId);

            var viewModel = new CartViewModel();
            var needsSave = false;

            foreach (var item in cartItems)
            {
                var product = item.Product;

                // The product was deleted since it was added to the cart.
                if (product == null)
                {
                    _unitOfWork.Carts.Remove(item);
                    needsSave = true;
                    continue;
                }

                var effectiveQuantity = Math.Min(item.Quantity, product.StockQuantity);

                // No stock left at all — drop the item from the cart.
                if (effectiveQuantity <= 0)
                {
                    _unitOfWork.Carts.Remove(item);
                    needsSave = true;
                    continue;
                }

                // Stock shrank since the item was added — clamp it and persist the change.
                if (effectiveQuantity != item.Quantity)
                {
                    item.Quantity = effectiveQuantity;
                    _unitOfWork.Carts.Update(item);
                    needsSave = true;
                }

                viewModel.Items.Add(new CartItemViewModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductImageUrl = product.ImageUrl,
                    UnitPrice = product.Price,
                    Quantity = effectiveQuantity,
                    AvailableStock = product.StockQuantity
                });
            }

            if (needsSave)
            {
                await _unitOfWork.SaveChangesAsync();
            }

            return viewModel;
        }

        public async Task<int> GetItemCountAsync()
        {
            var cartItems = await _unitOfWork.Carts.GetByCustomerIdAsync(CustomerId);
            return cartItems.Sum(i => i.Quantity);
        }

        public async Task AddToCartAsync(int productId, int quantity)
        {
            if (quantity <= 0)
            {
                throw new BusinessRuleException("Quantity must be at least 1.");
            }

            var product = await _unitOfWork.Products.GetByIdAsync(productId)
                ?? throw new NotFoundException(nameof(Product), productId);

            var customerId = CustomerId;
            var existing = await _unitOfWork.Carts.GetItemAsync(customerId, productId);

            var newQuantity = (existing?.Quantity ?? 0) + quantity;

            if (newQuantity > product.StockQuantity)
            {
                throw new BusinessRuleException(
                    $"Only {product.StockQuantity} unit(s) of '{product.Name}' available.");
            }

            if (existing != null)
            {
                existing.Quantity = newQuantity;
                _unitOfWork.Carts.Update(existing);
            }
            else
            {
                await _unitOfWork.Carts.AddAsync(new CartItem
                {
                    CustomerId = customerId,
                    ProductId = productId,
                    Quantity = newQuantity
                });
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(int productId, int quantity)
        {
            var customerId = CustomerId;
            var existing = await _unitOfWork.Carts.GetItemAsync(customerId, productId)
                ?? throw new NotFoundException("Cart item", productId);

            if (quantity <= 0)
            {
                _unitOfWork.Carts.Remove(existing);
                await _unitOfWork.SaveChangesAsync();
                return;
            }

            var product = await _unitOfWork.Products.GetByIdAsync(productId)
                ?? throw new NotFoundException(nameof(Product), productId);

            if (quantity > product.StockQuantity)
            {
                throw new BusinessRuleException(
                    $"Only {product.StockQuantity} unit(s) of '{product.Name}' available.");
            }

            existing.Quantity = quantity;
            _unitOfWork.Carts.Update(existing);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(int productId)
        {
            var existing = await _unitOfWork.Carts.GetItemAsync(CustomerId, productId);

            if (existing != null)
            {
                _unitOfWork.Carts.Remove(existing);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync()
        {
            await _unitOfWork.Carts.RemoveRangeByCustomerIdAsync(CustomerId);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
