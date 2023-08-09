using System.ComponentModel.DataAnnotations;

namespace Eshop.Domain.Entities
{
    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public User? User { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        public int Count { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}