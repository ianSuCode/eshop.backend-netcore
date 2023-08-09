using Eshop.Domain.Dtos;
using Eshop.Domain.Entities;
using Eshop.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Data.Repositories
{
    public class CartItemRepository : GenericRepository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(EshopDbContext context) : base(context)
        {
        }

        public async Task<bool> CreateOrUpdateItemAsync(CartItemCountDto cartItemCountDto, int userId)
        {
            var product = await _context.Product.FindAsync(cartItemCountDto.ProductId);
            if (product == null) return false;

            var now = DateTime.Now;
            var cartItem = await _context.CartItem.Where(it => it.UserId == userId && it.ProductId == cartItemCountDto.ProductId).FirstOrDefaultAsync();
            if (cartItem == null)
            {
                await _context.CartItem.AddAsync(new CartItem
                {
                    UserId = userId,
                    ProductId = cartItemCountDto.ProductId,
                    Count = cartItemCountDto.Count,
                    CreatedAt = now,
                    UpdatedAt = now,
                });
            }
            else
            {
                cartItem.Count = cartItemCountDto.Count;
                cartItem.UpdatedAt = now;
            }
            _context.SaveChanges();
            return true;
        }

        public async Task<List<CartItem>> GetPersonalCartItemsAsync(int userId)
        {
            var q = from ci in _context.CartItem
                    join p in _context.Product on ci.ProductId equals p.Id into pGroup
                    from p in pGroup.DefaultIfEmpty()
                    where ci.UserId == userId
                    select new CartItem
                    {
                        Id = ci.Id,
                        ProductId = ci.ProductId,
                        Count = ci.Count,
                        Product = new Product
                        {
                            Id = ci.ProductId,
                            Name = p.Name,
                            CategoryId = p.CategoryId,
                            Description = p.Description,
                            Price = p.Price,
                        },
                    };
            return await q.ToListAsync();
        }

        public async Task<bool> DeleteAsync(int userId, int productId)
        {
            var item = _context.CartItem.Where(it => it.UserId == userId && it.ProductId == productId).FirstOrDefault();
            if (item != null)
            {
                _context.CartItem.Remove(item);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task ClearAsync(int userId)
        {
            var items = _context.CartItem.Where(it => it.UserId == userId);
            _context.CartItem.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}