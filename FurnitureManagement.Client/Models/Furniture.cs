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
        public int? CategoryId { get; set; }
        
        /// <summary>
        /// 型号
        /// </summary>
        public string? Model { get; set; }
        
        /// <summary>
        /// 材质
        /// </summary>
        public string? Material { get; set; }
        
        /// <summary>
        /// 颜色
        /// </summary>
        public string? Color { get; set; }
        
        /// <summary>
        /// 进货价格
        /// </summary>
        public decimal PurchasePrice { get; set; }
        
        /// <summary>
        /// 销售价格
        /// </summary>
        public decimal SalePrice { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// 创建人ID
        /// </summary>
        public int? CreatedBy { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 所属分类（导航属性，仅客户端使用）
        /// </summary>
        public Category? Category { get; set; }
    }
}
