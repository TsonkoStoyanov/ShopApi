namespace ShopApi.Data.Models
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public string Status { get; set; }
    }
}