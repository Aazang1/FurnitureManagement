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
        /// 仓库地址
        /// </summary>
        public string Address { get; set; } = string.Empty;
        
        /// <summary>
        /// 联系人
        /// </summary>
        public string ContactPerson { get; set; } = string.Empty;
        
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; } = string.Empty;
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}