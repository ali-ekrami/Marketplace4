using Microsoft.AspNetCore.Identity;
using tagr.Exceptions;
using tagr.Mapping;
using tagr.Models;
using tagr.Services.Interfaces;
using tagr.UnitOfWork;
using tagr.ViewModels;

namespace tagr.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<List<UserAdminListItemViewModel>> GetAllAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return users.ToAdminListItemViewModels();
        }

        public async Task ToggleStatusAsync(string id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(ApplicationUser), id);

            user.IsSuspended = !user.IsSuspended;

            // Rotating the security stamp drops any session the user already has open,
            // so a suspension is not just a block on the next sign-in.
            await _userManager.UpdateSecurityStampAsync(user);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
