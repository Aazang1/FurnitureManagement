using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureManagement.Server.Models
{
    /// <summary>
    /// 采购订单模型
    /// </summary>
    [Table("purchase_order")] // 添加表名映射
    public class PurchaseOrder
    {
        /// <summary>
        /// 采购订单ID - 对应数据库的 purchase_id
        /// </summary>
        [Column("purchase_id")]
        public int PurchaseOrderId { get; set; }

        /// <summary>
        /// 供应商ID - 对应数据库的 supplier_id
        /// </summary>
        [Column("supplier_id")]
        public int SupplierId { get; set; }

        /// <summary>
        /// 采购日期 - 对应数据库的 purchase_date
        /// </summary>
        [Column("purchase_date")]
        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 采购总金额 - 对应数据库的 total_amount
        /// 注意：数据库类型是 decimal(12,2)
        /// </summary>
        [Column("total_amount", TypeName = "decimal(12,2)")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 订单状态 (pending/completed/cancelled) - 对应数据库的 status
        /// 注意：数据库类型是 enum
        /// </summary>
        [Column("status")]
        public string Status { get; set; } = "pending";

        /// <summary>
        /// 创建人ID - 对应数据库的 created_by
        /// 这是图片中显示但模型缺少的重要字段！
        /// </summary>
        [Column("created_by")]
        public int CreatedBy { get; set; }

        /// <summary>
        /// 创建时间 - 对应数据库的 created_at
        /// 注意：数据库类型是 timestamp
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 供应商信息
        /// </summary>
        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }

        /// <summary>
        /// 创建人用户信息
        /// </summary>
        [ForeignKey("CreatedBy")]
        public User? CreatedByUser { get; set; }

        /// <summary>
        /// 采购明细列表
        /// </summary>
        public List<PurchaseDetail>? PurchaseDetails { get; set; }

        /// <summary>
        /// 计算采购订单的总金额
        /// </summary>

    }
}