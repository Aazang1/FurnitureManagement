namespace FurnitureManagement.Client.Models
{
    /// <summary>
    /// 仓库模型
    /// </summary>
    public class Warehouse
    {
        /// <summary>
        /// 仓库ID
        /// </summary>
        public int WarehouseId { get; set; }
        
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WarehouseName { get; set; } = string.Empty;
        
        /// <summary>
        /// 位置
        /// </summary>
        public string? Location { get; set; }
        
        /// <summary>
        /// 容量
        /// </summary>
        public int? Capacity { get; set; }
        
        /// <summary>
        /// 管理员
        /// </summary>
        public string? Manager { get; set; }
        
        /// <summary>
        /// 创建人ID
        /// </summary>
        public int? CreatedBy { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}