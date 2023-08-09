using Eshop.Data.Repositories;
using Eshop.Domain;
using Eshop.Domain.Interfaces;

namespace Eshop.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EshopDbContext _context;

        private CartItemRepository? _cartItemRepository;
        private CategoryRepository? _categoryRepository;
        private OrderRepository? _orderRepository;
        private ProductRepository? _productRepository;
        private UserRepository? _userRepository;
        private UserRoleRepository? _userRoleRepository;

        public UnitOfWork(EshopDbContext context)
        {
            _context = context;
        }

        #region Repositories

        public ICartItemRepository CartItemRepository
        {
            get
            {
                if (_cartItemRepository == null)
                {
                    _cartItemRepository = new CartItemRepository(_context);
                }
                return _cartItemRepository;
            }
        }

        public ICategoryRepository CategoryRepository
        {
            get
            {
                if (_categoryRepository == null)
                {
                    _categoryRepository = new CategoryRepository(_context);
                }
                return _categoryRepository;
            }
        }

        public IOrderRepository OrderRepository
        {
            get
            {
                if (_orderRepository == null)
                {
                    _orderRepository = new OrderRepository(_context);
                }
                return _orderRepository;
            }
        }

        public IProductRepository ProductRepository
        {
            get
            {
                if (_productRepository == null)
                {
                    _productRepository = new ProductRepository(_context);
                }
                return _productRepository;
            }
        }

        public IUserRepository UserRepository
        {
            get
            {
                if (_userRepository == null)
                {
                    _userRepository = new UserRepository(_context);
                }
                return _userRepository;
            }
        }

        public IUserRoleRepository UserRoleRepository
        {
            get
            {
                if (_userRoleRepository == null)
                {
                    _userRoleRepository = new UserRoleRepository(_context);
                }
                return _userRoleRepository;
            }
        }

        #endregion Repositories

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public Task<int> CompleteAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}