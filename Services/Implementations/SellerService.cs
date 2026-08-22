using Microsoft.AspNetCore.Identity;
using tagr.Exceptions;
using tagr.Mapping;
using tagr.Models;
using tagr.Services.Interfaces;
using tagr.UnitOfWork;
using tagr.ViewModels;

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

        public async Task<List<SellerRequestListItemViewModel>> GetPendingRequestsAsync()
        {
            var users = await _unitOfWork.Users.GetPendingSellerRequestsAsync();
            return users.ToSellerRequestViewModels();
        }

        public async Task<SellerStatusViewModel> GetStatusAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException(nameof(ApplicationUser), userId);

            return user.ToSellerStatusViewModel();
        }

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

            // A rejected user must not keep selling rights granted by an earlier approval.
            if (await _userManager.IsInRoleAsync(user, "Seller"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Seller");
            }
        }
        public async Task RequestAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException(nameof(ApplicationUser), userId);

            if (user.IsSellerApproved)
                throw new BusinessRuleException("You are already an approved seller.");

            if (user.IsSellerRequested)
                throw new BusinessRuleException("You already have a pending seller request.");

            user.IsSellerRequested = true;
            await _userManager.UpdateAsync(user);
        }
    }
}
