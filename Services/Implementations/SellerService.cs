using Microsoft.AspNetCore.Identity;
using tagr.Exceptions;
using tagr.Models;
using tagr.Services.Interfaces;
using tagr.UnitOfWork;

namespace tagr.Services.Implementations
{
    public class SellerService : ISellerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public SellerService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public Task<List<ApplicationUser>> GetPendingRequestsAsync()
            => _unitOfWork.Users.GetPendingSellerRequestsAsync();

        public async Task ApproveAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException(nameof(ApplicationUser), userId);

            if (!user.IsSellerRequested)
            {
                throw new BusinessRuleException("This user has not requested to become a seller.");
            }

            user.IsSellerApproved = true;
            user.IsSellerRequested = false;

            await _userManager.UpdateAsync(user);

            if (!await _userManager.IsInRoleAsync(user, "Seller"))
            {
                await _userManager.AddToRoleAsync(user, "Seller");
            }
        }

        public async Task RejectAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException(nameof(ApplicationUser), userId);

            user.IsSellerRequested = false;
            user.IsSellerApproved = false;

            await _userManager.UpdateAsync(user);
        }
    }
}