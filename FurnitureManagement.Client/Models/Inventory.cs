namespace FurnitureManagement.Client.Models
{
    /// <summary>
    /// 库存模型
    /// </summary>
    public class Inventory
    {
        /// <summary>
        /// 库存ID
        /// </summary>
        public int InventoryId { get; set; }
        
        /// <summary>
        /// 商品ID
        /// </summary>
        public int FurnitureId { get; set; }
        
        /// <summary>
        /// 仓库ID
        /// </summary>
        public int WarehouseId { get; set; }
        
        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }
        
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 商品信息
        /// </summary>
        public Furniture? Furniture { get; set; }
        
        /// <summary>
        /// 仓库信息
        /// </summary>
        public Warehouse? Warehouse { get; set; }
    }
}