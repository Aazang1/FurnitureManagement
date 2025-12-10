using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureManagement.Server.Models
{
    public class Inventory
    {
        [Key]
        [Column("inventory_id")]
        public int InventoryId { get; set; }

        [Column("furniture_id")]
        public int? FurnitureId { get; set; }

        [Column("warehouse_id")]
        public int? WarehouseId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; } = 0;

        [Column("min_stock_level")]
        public int? MinStockLevel { get; set; } = 10;

        [Column("last_updated")]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // 导航属性
        [ForeignKey("FurnitureId")]
        public virtual Furniture? Furniture { get; set; }

        [ForeignKey("WarehouseId")]
        public virtual Warehouse? Warehouse { get; set; }
    }
}