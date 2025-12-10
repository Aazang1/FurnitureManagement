namespace FurnitureManagement.Client.Models
{
    /// <summary>
    /// 供应商模型
    /// </summary>
    public class Supplier
    {
        /// <summary>
        /// 供应商ID
        /// </summary>
        public int SupplierId { get; set; }
        
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; } = string.Empty;
        
        /// <summary>
        /// 联系人
        /// </summary>
        public string ContactPerson { get; set; } = string.Empty;
        
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; } = string.Empty;
        
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; } = string.Empty;
        
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