using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QE180082_Ass1_Product.Models.Entities
{
    [Table("order_items")]
    public class OrderItemEntity
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        
        [Column("order_id")]
        public int OrderId { get; set; }
        
        [Column("product_id")]
        public int ProductId { get; set; }
        
        [Column("quantity")]
        public int Quantity { get; set; }
        
        [Column("price")]
        public decimal Price { get; set; }
        
        // Navigation properties
        [ForeignKey("OrderId")]
        public OrderEntity Order { get; set; } = null!;
        
        [ForeignKey("ProductId")]
        public ProductEntity Product { get; set; } = null!;
    }
}
