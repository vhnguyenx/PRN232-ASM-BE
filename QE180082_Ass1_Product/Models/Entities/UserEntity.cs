using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QE180082_Ass1_Product.Models.Entities
{
    [Table("users")]
    public class UserEntity
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        
        //[Column("supabase_user_id")]
        //public string? SupabaseUserId { get; set; }
        
        [Column("email")]
        public required string Email { get; set; }
        
        [Column("password")]
        public string? Password { get; set; }

        //[Column("full_name")]
        //public string? FullName { get; set; }

        //[Column("phone")]
        //public string? Phone { get; set; }

        //[Column("address")]
        //public string? Address { get; set; }

        //[Column("role")]
        //public string Role { get; set; } = "user";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //[Column("updated_at")]
        //public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<CartItemEntity> CartItems { get; set; } = new List<CartItemEntity>();
        public ICollection<OrderEntity> Orders { get; set; } = new List<OrderEntity>();
    }
}
