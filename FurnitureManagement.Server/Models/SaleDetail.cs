using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FurnitureManagement.Server.Models
{
    public class SaleDetail
    {
        [Key]
        [Column("detail_id")]
        public int DetailId { get; set; }

        [Column("sale_id")]
        public int SaleId { get; set; }

        [Column("furniture_id")]
        public int FurnitureId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("unit_price")]
        public decimal UnitPrice { get; set; }

        [Column("total_price")]
        public decimal TotalPrice { get; set; }

        [Column("warehouse_id")]
        public int WarehouseId { get; set; }

        // Navigation properties
        [JsonIgnore]
        public SaleOrder? SaleOrder { get; set; }
        public Furniture? Furniture { get; set; }
        public Warehouse? Warehouse { get; set; }
    }
}