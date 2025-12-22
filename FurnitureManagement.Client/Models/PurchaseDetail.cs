namespace FurnitureManagement.Client.Models
{
    /// <summary>
    /// 采购明细模型
    /// </summary>
    public class PurchaseDetail
    {
        /// <summary>
        /// 采购明细ID
        /// </summary>
        public int PurchaseDetailId { get; set; }
        
        /// <summary>
        /// 采购订单ID
        /// </summary>
        public int PurchaseOrderId { get; set; }
        
        /// <summary>
        /// 商品ID
        /// </summary>
        public int FurnitureId { get; set; }
        
        /// <summary>
        /// 入库仓库ID
        /// </summary>
        public int WarehouseId { get; set; }
        
        /// <summary>
        /// 采购数量
        /// </summary>
        public int Quantity { get; set; }
        
        /// <summary>
        /// 采购单价
        /// </summary>
        public decimal UnitPrice { get; set; }
        
        /// <summary>
        /// 明细金额
        /// </summary>
        public decimal Amount => Quantity * UnitPrice;
        
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