namespace Eshop.Domain.Dtos
{
    public class OrderInfoDto : OrderStateDto
    {
        public int Count { get; set; }
        public int ProductId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ProductNamePriceDto? Product { get; set; }
    }
}