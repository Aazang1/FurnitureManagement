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
        /// <returns>库存汇总数据</returns>
        [HttpGet("inventory-summary")]
        public async Task<IActionResult> GetInventorySummary()
        {
            var inventorySummary = await _context.InventorySummary.ToListAsync();
            return Ok(inventorySummary);
        }

        /// <summary>
        /// 获取销售日报
        /// </summary>
        /// <returns>销售日报数据</returns>
        [HttpGet("sales-daily")]
        public async Task<IActionResult> GetSalesDaily()
        {
            var salesDaily = await _context.SalesDaily.ToListAsync();
            return Ok(salesDaily);
        }

        /// <summary>
        /// 获取人员操作报表
        /// </summary>
        /// <returns>人员操作数据</returns>
        [HttpGet("user-operations")]
        public async Task<IActionResult> GetUserOperations()
        {
            var userOperations = await _context.UserOperations.ToListAsync();
            return Ok(userOperations);
        }
    }
}