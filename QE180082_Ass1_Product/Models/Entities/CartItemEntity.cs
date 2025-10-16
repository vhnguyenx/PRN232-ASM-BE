using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QE180082_Ass1_Product.Models.Entities
{
    [Table("cart_items")]
    public class CartItemEntity
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        
        [Column("user_id")]
        public int UserId { get; set; }
        
        [Column("product_id")]
        public int ProductId { get; set; }
        
        [Column("quantity")]
        public int Quantity { get; set; }
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        [ForeignKey("UserId")]
        public UserEntity User { get; set; } = null!;
        
        [ForeignKey("ProductId")]
        public ProductEntity Product { get; set; } = null!;
    }
}
