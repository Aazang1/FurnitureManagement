using FurnitureManagement.Server.Data;
using FurnitureManagement.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureManagement.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取库存管理报表
        /// </summary>
        /// <returns>库存汇总数据（按商品汇总所有仓库库存）</returns>
        [HttpGet("inventory-summary")]
        public async Task<IActionResult> GetInventorySummary()
        {
            try
            {
                // 直接从优化后的视图获取数据，该视图已经按商品汇总了所有仓库的库存
                var inventorySummary = await _context.InventorySummary
                    .OrderBy(i => i.FurnitureId)
                    .ToListAsync();
                
                return Ok(inventorySummary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"获取库存管理报表失败: {ex.Message}" });
            }
        }

        /// <summary>
        /// 获取库存管理报表（按仓库分类）
        /// </summary>
        /// <returns>按仓库分组的库存数据</returns>
        [HttpGet("inventory-by-warehouse")]
        public async Task<IActionResult> GetInventoryByWarehouse()
        {
            try
            {
                // 使用原生SQL查询，按仓库分组获取库存数据
                var sql = @"
                    SELECT 
                        f.furniture_id AS FurnitureId,
                        f.furniture_name AS FurnitureName,
                        c.category_name AS CategoryName,
                        w.warehouse_name AS WarehouseName,
                        i.quantity AS Quantity,
                        f.purchase_price AS PurchasePrice,
                        f.sale_price AS SalePrice,
                        i.quantity * f.purchase_price AS InventoryCost,
                        i.quantity * f.sale_price AS InventoryValue
                    FROM inventory i
                    JOIN furniture f ON i.furniture_id = f.furniture_id
                    JOIN category c ON f.category_id = c.category_id
                    JOIN warehouse w ON i.warehouse_id = w.warehouse_id
                    ORDER BY f.furniture_id, w.warehouse_name";
                
                var inventoryByWarehouse = await _context.Database
                    .SqlQueryRaw<InventorySummary>(sql)
                    .ToListAsync();
                
                return Ok(inventoryByWarehouse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"获取按仓库分类的库存数据失败: {ex.Message}" });
            }
        }

        /// <summary>
        /// 获取销售日报
        /// </summary>
        /// <returns>销售日报数据</returns>
        [HttpGet("sales-daily")]
        public async Task<IActionResult> GetSalesDaily()
        {
            try
            {
                var salesDaily = await _context.SalesDaily
                    .OrderBy(s => s.SaleDay)
                    .ToListAsync();
                
                return Ok(salesDaily);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"获取销售日报失败: {ex.Message}" });
            }
        }

        /// <summary>
        /// 获取人员操作报表
        /// </summary>
        /// <returns>人员操作数据</returns>
        [HttpGet("user-operations")]
        public async Task<IActionResult> GetUserOperations()
        {
            try
            {
                var userOperations = await _context.UserOperations
                    .OrderBy(u => u.UserId)
                    .ToListAsync();
                
                return Ok(userOperations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"获取人员操作报表失败: {ex.Message}" });
            }
        }
    }
}