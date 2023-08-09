using Eshop.Domain.Dtos;
using Eshop.Domain.Entities;

namespace Eshop.Domain.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task PlaceAsync(int userId, int[] productIds);

        Task<IEnumerable<OrderInfoDto>> GetPersonalOrders(int userId);

        Task<IEnumerable<UserOrderDto>> GetAllUserOrders();
        Task<bool> ChangeStateAsync(OrderStateDto orderStateDto);
    }
}