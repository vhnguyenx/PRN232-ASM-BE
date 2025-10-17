using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QE180082_Ass1_Product.Models.Entities
{
    [Table("orders")]
    public class OrderEntity
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        
        [Column("user_id")]
        public int UserId { get; set; }
        
        [Column("total_amount")]
        public int TotalAmount { get; set; }
        
        [Column("status")]
        public required string Status { get; set; }
        
        [Column("payment_method")]
        public string? PaymentMethod { get; set; }
        
        [Column("payment_status")]
        public string PaymentStatus { get; set; } = "pending";
        
        [Column("shipping_address")]
        public string? ShippingAddress { get; set; }
        
        [Column("phone")]
        public string? Phone { get; set; }
        
        [Column("notes")]
        public string? Notes { get; set; }
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        //[Column("updated_at")]
        //public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        [ForeignKey("UserId")]
        public UserEntity User { get; set; } = null!;
        
        public ICollection<OrderItemEntity> OrderItems { get; set; } = new List<OrderItemEntity>();
    }
}
