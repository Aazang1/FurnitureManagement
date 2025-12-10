namespace FurnitureManagement.Client.Models
{
    /// <summary>
    /// 商品模型
    /// </summary>
    public class Furniture
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public int FurnitureId { get; set; }
        
        /// <summary>
        /// 商品名称
        /// </summary>
        public string FurnitureName { get; set; } = string.Empty;
        
        /// <summary>
        /// 分类ID
        /// </summary>
        public int CategoryId { get; set; }
        
        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// 图片URL
        /// </summary>
        public string? ImageUrl { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 所属分类
        /// </summary>
        public Category? Category { get; set; }
    }
}