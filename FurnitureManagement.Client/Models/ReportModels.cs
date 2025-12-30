namespace FurnitureManagement.Client.Models
{
    /// <summary>
    /// 库存管理报表模型
    /// </summary>
    public class InventorySummary
    {
        public int FurnitureId { get; set; }
        public string FurnitureName { get; set; }
        public string CategoryName { get; set; }
        public string WarehouseName { get; set; }
        public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal InventoryCost { get; set; }
        public decimal InventoryValue { get; set; }
    }

    /// <summary>
    /// 销售日报模型
    /// </summary>
    public class SalesDaily
    {
        public DateTime SaleDay { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalDiscount { get; set; }
    }

    /// <summary>
    /// 人员操作报表模型
    /// </summary>
    public class UserOperations
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string RealName { get; set; }
        public string Role { get; set; }
        public int PurchaseOrders { get; set; }
        public int SaleOrders { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}