using Eshop.Domain.Entities;
using Eshop.Domain.Interfaces;

namespace Eshop.Data.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(EshopDbContext context) : base(context)
        {
        }
    }
}