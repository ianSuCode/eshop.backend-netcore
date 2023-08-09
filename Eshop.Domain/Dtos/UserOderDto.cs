namespace Eshop.Domain.Dtos
{
    public class UserOrderDto
    {
        public int UserId { get; set; }
        public string? UserEmail { get; set; }

        public List<OrderInfoDto>? Orders { get; set; }
    }
}