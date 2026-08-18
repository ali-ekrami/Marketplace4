using tagr.ViewModels;

namespace tagr.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserAdminListItemViewModel>> GetAllAsync();
        Task ToggleStatusAsync(string id);
    }
}