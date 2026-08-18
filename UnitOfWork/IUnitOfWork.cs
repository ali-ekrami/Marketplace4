using tagr.Repositories.Interfaces;

namespace tagr.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        IUserRepository Users { get; }
        //IOrderRepository Orders { get; }
        //IUserRepository Users { get; }
        //ICartRepository Carts { get; }
        //IReviewRepository Reviews { get; }
        Task<int> SaveChangesAsync();
    }
}
