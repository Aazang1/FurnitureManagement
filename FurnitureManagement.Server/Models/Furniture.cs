using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureManagement.Server.Models
{
    public class Furniture
    {
        [Key]
        [Column("furniture_id")]
        public int FurnitureId { get; set; }

        [Column("furniture_name")]
        public string FurnitureName { get; set; } = string.Empty;

        [Column("category_id")]
        public int? CategoryId { get; set; }

        [Column("model")]
        public string? Model { get; set; }

        [Column("material")]
        public string? Material { get; set; }

        [Column("color")]
        public string? Color { get; set; }

        [Column("purchase_price")]
        public decimal PurchasePrice { get; set; }

        [Column("sale_price")]
        public decimal SalePrice { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("created_by")]
        public int? CreatedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public Category? Category { get; set; }
    }
}