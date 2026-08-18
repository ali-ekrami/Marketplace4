using tagr.Data;
using tagr.Repositories.Interfaces;
using tagr.Repositories.Implementations;

namespace tagr.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ICategoryRepository Categories { get; }
        public IProductRepository Products { get; }
        //public IOrderRepository Orders { get; }
        //public IUserRepository Users { get; }
        //public ICartRepository Carts { get; }
        //public IReviewRepository Reviews { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Categories = new CategoryRepository(_context);
            Products = new ProductRepository(_context);
            //Orders = new OrderRepository(_context);
            //Users = new UserRepository(_context);
            //Carts = new CartRepository(_context);
            //Reviews = new ReviewRepository(_context);
        }
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
    }
}
