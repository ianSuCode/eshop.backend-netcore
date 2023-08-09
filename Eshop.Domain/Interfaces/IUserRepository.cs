using Eshop.Domain.Dtos;
using Eshop.Domain.Entities;

namespace Eshop.Domain.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> FindByEmailAsync(string email);

        Task<bool> IsExisted(string email);

        Task SignupAsync(UserLoginDto loginDto);

        Task<IEnumerable<UserViewDto>> GetAllInfosAsync();

        Task<bool> ChangeActiveAsync(UserActiveDto userActiveDto);

        Task<bool> DeleteCompletelyAsync(int id);
    }
}