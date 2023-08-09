namespace Eshop.Domain.Dtos
{
    public class UserViewDto
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public bool Active { get; set; }
        public List<string>? Roles { get; set; }

        public List<OrderStateDto>? Orders { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}