using Eshop.Domain.Enums;

namespace Eshop.Domain.Entities
{
    public class UserRole
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public EnumRole Role { get; set; }
    }
}