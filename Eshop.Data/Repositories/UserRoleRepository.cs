using Eshop.Domain.Entities;
using Eshop.Domain.Interfaces;

namespace Eshop.Data.Repositories
{
    public class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(EshopDbContext context) : base(context)
        {
        }
    }
}