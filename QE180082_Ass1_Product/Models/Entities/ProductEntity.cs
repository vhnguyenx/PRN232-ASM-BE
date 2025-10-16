using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QE180082_Ass1_Product.Models.Entities
{
    [Table("product")]
    public class ProductEntity
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        
        [Column("name")]
        public required string Name { get; set; }
        
        [Column("description")]
        public required string Description { get; set; }
        
        [Column("price")]
        public required decimal Price { get; set; }
        
        [Column("image")]
        public string? Image { get; set; }
        
        //[Column("category")]
        //public string? Category { get; set; }
        
        //[Column("stock")]
        //public int Stock { get; set; } = 0;
        
        //[Column("created_by")]
        //public int? CreatedBy { get; set; }
        
        //[Column("created_at")]
        //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        //[Column("updated_at")]
        //public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public ICollection<CartItemEntity> CartItems { get; set; } = new List<CartItemEntity>();
        public ICollection<OrderItemEntity> OrderItems { get; set; } = new List<OrderItemEntity>();
    }
}
