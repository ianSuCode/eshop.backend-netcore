using Eshop.Domain.Dtos;
using Eshop.Domain.Entities;

namespace Eshop.Domain.Interfaces
{
    public interface ICartItemRepository : IGenericRepository<CartItem>
    {
        Task<bool> CreateOrUpdateItemAsync(CartItemCountDto cartItemCountDto, int userId);

        Task<List<CartItem>> GetPersonalCartItemsAsync(int userId);

        Task<bool> DeleteAsync(int userId, int productId);
        Task ClearAsync(int userId);
    }
}