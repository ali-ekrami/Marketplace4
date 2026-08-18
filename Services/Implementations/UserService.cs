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

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            await _unitOfWork.SaveChangesAsync();
        }
    }
}