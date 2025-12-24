using FurnitureManagement.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace FurnitureManagement.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public TestController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Test/database
        [HttpGet("database")]
        public async Task<ActionResult> TestDatabase()
        {
            try
            {
                // 测试Entity Framework连接
                var canConnect = await _context.Database.CanConnectAsync();
                if (!canConnect)
                {
                    return Ok(new { success = false, message = "Entity Framework无法连接到数据库" });
                }

                // 测试直接MySQL连接
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                // 检查supplier表
                using var command = new MySqlCommand("SELECT COUNT(*) FROM supplier", connection);
                var count = await command.ExecuteScalarAsync();

                return Ok(new 
                { 
                    success = true, 
                    message = "数据库连接成功",
                    supplierCount = count,
                    connectionString = connectionString?.Replace(_configuration.GetConnectionString("DefaultConnection")?.Split("password=")[1]?.Split(";")[0] ?? "", "***")
                });
            }
            catch (Exception ex)
            {
                return Ok(new 
                { 
                    success = false, 
                    message = $"数据库连接失败: {ex.Message}",
                    details = ex.ToString()
                });
            }
        }

        // GET: api/Test/supplier
        [HttpGet("supplier")]
        public async Task<ActionResult> TestSupplier()
        {
            try
            {
                // 使用Entity Framework查询
                var suppliers = await _context.Supplier.Take(3).ToListAsync();
                
                return Ok(new 
                { 
                    success = true, 
                    message = "供应商查询成功",
                    count = suppliers.Count,
                    data = suppliers
                });
            }
            catch (Exception ex)
            {
                return Ok(new 
                { 
                    success = false, 
                    message = $"供应商查询失败: {ex.Message}",
                    details = ex.ToString()
                });
            }
        }

        // GET: api/Test/supplier-raw
        [HttpGet("supplier-raw")]
        public async Task<ActionResult> TestSupplierRaw()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new MySqlCommand("SELECT * FROM supplier LIMIT 3", connection);
                using var reader = await command.ExecuteReaderAsync();

                var results = new List<object>();
                while (await reader.ReadAsync())
                {
                    var item = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        item[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    }
                    results.Add(item);
                }

                return Ok(new 
                { 
                    success = true, 
                    message = "原始SQL查询成功",
                    count = results.Count,
                    data = results
                });
            }
            catch (Exception ex)
            {
                return Ok(new 
                { 
                    success = false, 
                    message = $"原始SQL查询失败: {ex.Message}",
                    details = ex.ToString()
                });
            }
        }
    }
}
