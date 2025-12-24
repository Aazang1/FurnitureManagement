namespace FurnitureManagement.Client.Models
{
    /// <summary>
    /// 采购订单模型
    /// </summary>
    public class PurchaseOrder
    {
        /// <summary>
        /// 采购订单ID
        /// </summary>
        public int PurchaseOrderId { get; set; }
        
        /// <summary>
        /// 供应商ID
        /// </summary>
        public int SupplierId { get; set; }
        
        /// <summary>
        /// 采购日期
        /// </summary>
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 采购总金额
        /// </summary>
        public decimal TotalAmount { get; set; }
        
        /// <summary>
        /// 订单状态 (pending/completed/cancelled)
        /// </summary>
        public string Status { get; set; } = "pending";
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 供应商信息
        /// </summary>
        public Supplier? Supplier { get; set; }
        
        /// <summary>
        /// 采购明细列表
        /// </summary>
        public List<PurchaseDetail>? PurchaseDetails { get; set; }
    }
}