using Eshop.Domain.Dtos;
using Eshop.Domain.Entities;
using Eshop.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Data.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(EshopDbContext context) : base(context)
        {
        }

        private OrderInfoDto ConverToOrderInfoDto(Order o)
        {
            return new OrderInfoDto
            {
                Id = o.Id,
                State = o.State,
                UpdatedAt = o.UpdatedAt,
                CreatedAt = o.CreatedAt,
                Count = o.Count,
                ProductId = o.ProductId,
                Product = new ProductNamePriceDto
                {
                    Name = o.Product!.Name,
                    Price = o.Product.Price,
                }
            };
        }

        public async Task PlaceAsync(int userId, int[] productIds)
        {
            var carts = await _context.CartItem.Where(it => it.UserId == userId && productIds.Contains(it.ProductId)).ToListAsync();
            if (carts.Any())
            {
                var now = DateTime.Now;
                var newOrders = carts.Select(it => new Order { UserId = it.UserId, ProductId = it.ProductId, Count = it.Count, State = "new", CreatedAt = now, UpdatedAt = now });
                _context.Order.AddRange(newOrders);
                await _context.SaveChangesAsync();

                // remove carts
                _context.CartItem.RemoveRange(carts);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<OrderInfoDto>> GetPersonalOrders(int userId)
        {
            var orders = await _context.Order.Where(it => it.UserId == userId).Include(it => it.Product).ToListAsync();
            var result = orders.Select(it => ConverToOrderInfoDto(it));
            return result;
        }

        public async Task<IEnumerable<UserOrderDto>> GetAllUserOrders()
        {
            var orders = await _context.Order.Include(it => it.User).Include(it => it.Product).ToListAsync();
            var result = orders.GroupBy(it => it.UserId).ToList().Select(it => new UserOrderDto
            {
                UserId = it.Key,
                UserEmail = it.First().User!.Email,
                Orders = it.Select(o => ConverToOrderInfoDto(o)).ToList(),
            });

            return result;
        }

        public async Task<bool> ChangeStateAsync(OrderStateDto orderStateDto)
        {
            var order = await GetByIdAsync(orderStateDto.Id);
            if (order == null) return false;

            order.State = orderStateDto.State;
            order.UpdatedAt = DateTime.Now;

            return true;
        }
    }
}