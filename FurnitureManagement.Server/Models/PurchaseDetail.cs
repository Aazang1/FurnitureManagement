using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FurnitureManagement.Server.Models
{
    /// <summary>
    /// 采购明细模型
    /// </summary>
    [Table("purchase_details")] // 映射数据库表名
    public class PurchaseDetail
    {
        /// <summary>
        /// 采购明细ID - 对应数据库的 detail_id
        /// </summary>
        [Key]
        [Column("detail_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PurchaseDetailId { get; set; }

        /// <summary>
        /// 采购订单ID - 对应数据库的 purchase_id
        /// </summary>
        [Column("purchase_id")]
        public int PurchaseOrderId { get; set; }

        /// <summary>
        /// 商品ID - 对应数据库的 furniture_id
        /// </summary>
        [Column("furniture_id")]
        public int FurnitureId { get; set; }

        /// <summary>
        /// 入库仓库ID - 对应数据库的 warehouse_id
        /// </summary>
        [Column("warehouse_id")]
        public int WarehouseId { get; set; }

        /// <summary>
        /// 采购数量 - 对应数据库的 quantity
        /// </summary>
        [Column("quantity")]
        [Range(1, int.MaxValue, ErrorMessage = "采购数量必须大于0")]
        public int Quantity { get; set; }

        /// <summary>
        /// 采购单价 - 对应数据库的 unit_price
        /// 数据库类型为 decimal(10,2)
        /// </summary>
        [Column("unit_price", TypeName = "decimal(10,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "单价必须大于0")]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 明细金额 - 对应数据库的 total_price
        /// 数据库类型为 decimal(10,2)
        /// 注意：这里需要映射到数据库的 total_price 字段
        /// </summary>
        [Column("total_price", TypeName = "decimal(10,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "总金额必须大于0")]
        public decimal Amount { get; set; }

        // 计算属性，返回金额（只读，不映射到数据库）
        [NotMapped]
        public decimal CalculatedAmount => Quantity * UnitPrice;

        /// <summary>
        /// 采购订单信息
        /// </summary>
        [ForeignKey("PurchaseOrderId")]
        public PurchaseOrder? PurchaseOrder { get; set; }

        /// <summary>
        /// 商品信息
        /// </summary>
        [ForeignKey("FurnitureId")]
        public Furniture? Furniture { get; set; }

        /// <summary>
        /// 仓库信息
        /// </summary>
        [ForeignKey("WarehouseId")]
        public Warehouse? Warehouse { get; set; }

        /// <summary>
        /// 计算方法：更新 Amount 字段
        /// 应在保存前调用此方法
        /// </summary>
        public void CalculateAmount()
        {
            Amount = Quantity * UnitPrice;
        }
    }
}