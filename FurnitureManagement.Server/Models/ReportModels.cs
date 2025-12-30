using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureManagement.Server.Models
{
    /// <summary>
    /// 库存汇总视图模型
    /// </summary>
    public class InventorySummary
    {
        [Key]
        [Column("furniture_id")]
        public int FurnitureId { get; set; }
        
        [Column("furniture_name")]
        public string FurnitureName { get; set; }
        
        [Column("category_name")]
        public string CategoryName { get; set; }
        
        [Column("warehouse_name")]
        public string WarehouseName { get; set; }
        
        [Column("quantity")]
        public int Quantity { get; set; }
        
        [Column("purchase_price")]
        public decimal PurchasePrice { get; set; }
        
        [Column("sale_price")]
        public decimal SalePrice { get; set; }
        
        [Column("inventory_cost")]
        public decimal InventoryCost { get; set; }
        
        [Column("inventory_value")]
        public decimal InventoryValue { get; set; }
    }

    /// <summary>
    /// 销售日报视图模型
    /// </summary>
    public class SalesDaily
    {
        [Key]
        [Column("sale_day")]
        public DateTime SaleDay { get; set; }
        
        [Column("order_count")]
        public int OrderCount { get; set; }
        
        [Column("total_sales")]
        public decimal TotalSales { get; set; }
        
        [Column("total_discount")]
        public decimal TotalDiscount { get; set; }
    }

    /// <summary>
    /// 用户操作报表视图模型
    /// </summary>
    public class UserOperations
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }
        
        [Column("username")]
        public string Username { get; set; }
        
        [Column("real_name")]
        public string RealName { get; set; }
        
        [Column("role")]
        public string Role { get; set; }
        
        [Column("purchase_orders")]
        public int PurchaseOrders { get; set; }
        
        [Column("sale_orders")]
        public int SaleOrders { get; set; }
        
        [Column("last_login")]
        public DateTime? LastLogin { get; set; }
    }
}