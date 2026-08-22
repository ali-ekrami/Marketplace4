using tagr.Exceptions;
using tagr.Mapping;
using tagr.Models;
using tagr.Services.Interfaces;
using tagr.UnitOfWork;
using tagr.ViewModels;

namespace tagr.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ReviewListItemViewModel>> GetByProductIdAsync(int productId)
        {
            var reviews = await _unitOfWork.Reviews.GetByProductIdAsync(productId);
            return reviews.ToListItemViewModels();
        }

        public async Task CreateAsync(ReviewCreateViewModel model, string customerId)
        {
            _ = await _unitOfWork.Products.GetByIdAsync(model.ProductId)
                ?? throw new NotFoundException(nameof(Product), model.ProductId);

            if (!await _unitOfWork.Orders.HasPurchasedProductAsync(customerId, model.ProductId))
            {
                throw new BusinessRuleException("You can only review products you have purchased.");
            }

            if (await _unitOfWork.Reviews.ExistsForCustomerAsync(customerId, model.ProductId))
            {
                throw new BusinessRuleException("You have already reviewed this product.");
            }

            model.Comment = model.Comment.Trim();

            var review = model.ToEntity(customerId);
            review.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
