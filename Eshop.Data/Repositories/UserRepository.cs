using Eshop.Domain.Dtos;
using Eshop.Domain.Entities;
using Eshop.Domain.Enums;
using Eshop.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Data.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(EshopDbContext context) : base(context)
        {
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            return await _context.User.Where(it => it.Email == email).FirstOrDefaultAsync();
        }

        public async Task<bool> IsExisted(string email)
        {
            return (await FindByEmailAsync(email)) != null;
        }

        public async Task<UserInfoDto?> LoginAsync(string email, string hashedPwd)
        {
            var user = await _context.User.Where(it => it.Email == email && it.Password == hashedPwd && it.Active).FirstOrDefaultAsync();
            if (user != null)
            {
                var roles = (await _context.UserRole.Where(it => it.UserId == user.Id).ToListAsync()).Select(it => it.Role.ToString()).ToList();
                return new UserInfoDto
                {
                    Id = user.Id,
                    Email = email,
                    Roles = roles,
                };
            }
            return null;
        }

        public async Task SignupAsync(UserLoginDto loginDto)
        {
            await _context.User.AddAsync(new User { Email = loginDto.Email, Password = loginDto.Password });
            var newUser = (await FindByEmailAsync(loginDto.Email))!;
            await _context.UserRole.AddAsync(new UserRole { UserId = newUser.Id, Role = EnumRole.User });
        }

        public async Task<IEnumerable<UserViewDto>> GetAllInfosAsync()
        {
            var user = await _context.User.Include(x => x.Roles).ToListAsync();
            var orders = await _context.Order.ToListAsync();

            var result = from u in user
                         join o in orders on u.Id equals o.UserId into oGrp
                         select new UserViewDto
                         {
                             Id = u.Id,
                             Email = u.Email,
                             Active = u.Active,
                             Roles = u.Roles.Select(r => r.Role.ToString()).ToList(),
                             CreatedAt = u.CreatedAt,
                             Orders = oGrp.Select(og => new OrderStateDto { Id = og.Id, State = og.State }).ToList()
                         };

            return result;
        }

        public async Task<bool> ChangeActiveAsync(UserActiveDto userActiveDto)
        {
            var user = await _context.User.Where(it => it.Id == userActiveDto.Id).FirstOrDefaultAsync();
            if (user == null) return false;
            user.Active = userActiveDto.Active;
            user.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCompletelyAsync(int id)
        {
            var user = await GetByIdAsync(id);
            if (user == null) return false;

            var orders = await _context.Order.Where(it => it.UserId == id).ToListAsync();
            if (orders != null && orders.Count > 0) _context.Order.RemoveRange(orders);
            await _context.SaveChangesAsync();

            var cartItems = await _context.CartItem.Where(it => it.UserId == id).ToListAsync();
            if (cartItems != null && cartItems.Count > 0) _context.CartItem.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}