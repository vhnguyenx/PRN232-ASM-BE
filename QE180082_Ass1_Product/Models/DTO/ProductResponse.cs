namespace QE180082_Ass1_Product.Models.DTO
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal Price { get; set; }
        public string? Image { get; set; }
    }
}
