using Eshop.Domain.Interfaces;

namespace Eshop.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        ICartItemRepository CartItemRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IOrderRepository OrderRepository { get; }
        IProductRepository ProductRepository { get; }
        IUserRepository UserRepository { get; }
        IUserRoleRepository UserRoleRepository { get; }

        int Complete();

        Task<int> CompleteAsync();
    }
}