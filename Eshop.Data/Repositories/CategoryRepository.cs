using Eshop.Domain.Entities;
using Eshop.Domain.Interfaces;

namespace Eshop.Data.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(EshopDbContext context) : base(context)
        {
        }
    }
}