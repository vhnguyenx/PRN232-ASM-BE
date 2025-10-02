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
    }
}
