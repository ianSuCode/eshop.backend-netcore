using System.ComponentModel.DataAnnotations;

namespace Eshop.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public int Price { get; set; }
    }
}