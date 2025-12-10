namespace FurnitureManagement.Client.Models
{
    /// <summary>
    /// 商品分类模型
    /// </summary>
    public class Category
    {
        /// <summary>
        /// 分类ID
        /// </summary>
        public int CategoryId { get; set; }
        
        /// <summary>
        /// 分类名称
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;
        
        /// <summary>
        /// 分类描述
        /// </summary>
        public string? Description { get; set; }
        
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