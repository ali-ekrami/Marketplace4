using Microsoft.EntityFrameworkCore;
using tagr.Data;
using tagr.Models;
using tagr.Repositories.Interfaces;

namespace tagr.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ApplicationUser>> GetAllAsync()
            => await _context.Users
                .AsNoTracking()
                .OrderBy(u => u.FullName)
                .ToListAsync();

        public async Task<ApplicationUser?> GetByIdAsync(string id)
            => await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

        public async Task<List<ApplicationUser>> GetPendingSellerRequestsAsync()
            => await _context.Users
                .AsNoTracking()
                .Where(u => u.IsSellerRequested && !u.IsSellerApproved)
                .OrderBy(u => u.FullName)
                .ToListAsync();
    }
}