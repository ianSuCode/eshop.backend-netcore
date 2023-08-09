namespace Eshop.Domain.Dtos
{
    public class UserInfoDto
    {
        public int Id { get; set; }

        public string? Email { get; set; }

        public List<string>? Roles { get; set; }
    }
}