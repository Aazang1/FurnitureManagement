namespace FurnitureManagement.Client.Models
{
    /// <summary>
    /// 销售订单模型
    /// </summary>
    public class SaleOrder
    {
        /// <summary>
        /// 销售订单ID
        /// </summary>
        public int SaleId { get; set; }

        /// <summary>
        /// 客户姓名
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// 客户电话
        /// </summary>
        public string CustomerPhone { get; set; } = string.Empty;

        /// <summary>
        /// 销售日期
        /// </summary>
        public DateTime SaleDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        public decimal Discount { get; set; } = 0.00m;

        /// <summary>
        /// 最终金额
        /// </summary>
        public decimal FinalAmount { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; } = "pending";

        /// <summary>
        /// 创建人ID
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 销售明细
        /// </summary>
        public List<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
    }
}