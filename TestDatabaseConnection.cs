using System;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace FurnitureManagement.DatabaseTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== 数据库连接测试 ===");
            Console.WriteLine();
            
            string connectionString = "server=localhost;database=furniture_management;user=root;password=Y2486yzl;port=3306";
            
            Console.WriteLine("测试连接字符串:");
            Console.WriteLine($"服务器: localhost:3306");
            Console.WriteLine($"数据库: furniture_management");
            Console.WriteLine($"用户: root");
            Console.WriteLine($"密码: [已隐藏]");
            Console.WriteLine();
            
            // 测试基本连接
            await TestBasicConnection(connectionString);
            
            // 测试数据查询
            await TestDataQuery(connectionString);
            
            Console.WriteLine("\n=== 测试完成 ===");
            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
        }
        
        static async Task TestBasicConnection(string connectionString)
        {
            Console.WriteLine("1. 测试数据库连接:");
            
            try
            {
                using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();
                Console.WriteLine("   ✓ 数据库连接成功");
                
                // 测试数据库版本
                using var command = new MySqlCommand("SELECT VERSION()", connection);
                var version = await command.ExecuteScalarAsync();
                Console.WriteLine($"   ✓ MySQL版本: {version}");
                
                // 测试数据库是否存在
                command.CommandText = "SELECT DATABASE()";
                var database = await command.ExecuteScalarAsync();
                Console.WriteLine($"   ✓ 当前数据库: {database}");
                
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"   ✗ MySQL连接失败: {ex.Message}");
                Console.WriteLine($"   错误代码: {ex.Number}");
                
                switch (ex.Number)
                {
                    case 1045:
                        Console.WriteLine("   可能原因: 用户名或密码错误");
                        break;
                    case 1049:
                        Console.WriteLine("   可能原因: 数据库不存在");
                        break;
                    case 2003:
                        Console.WriteLine("   可能原因: MySQL服务未启动或端口错误");
                        break;
                    default:
                        Console.WriteLine($"   详细错误: {ex}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ✗ 连接失败: {ex.Message}");
            }
            Console.WriteLine();
        }
        
        static async Task TestDataQuery(string connectionString)
        {
            Console.WriteLine("2. 测试数据查询:");
            
            try
            {
                using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();
                
                // 检查表是否存在
                string[] tables = { "supplier", "category", "furniture", "user" };
                
                foreach (string table in tables)
                {
                    try
                    {
                        using var command = new MySqlCommand($"SELECT COUNT(*) FROM {table}", connection);
                        var count = await command.ExecuteScalarAsync();
                        Console.WriteLine($"   ✓ 表 '{table}' 存在，记录数: {count}");
                        
                        // 如果是supplier表，显示一些示例数据
                        if (table == "supplier" && Convert.ToInt32(count) > 0)
                        {
                            command.CommandText = "SELECT supplier_id, supplier_name, contact_person FROM supplier LIMIT 3";
                            using var reader = await command.ExecuteReaderAsync();
                            Console.WriteLine("     示例数据:");
                            while (await reader.ReadAsync())
                            {
                                Console.WriteLine($"       ID: {reader["supplier_id"]}, 名称: {reader["supplier_name"]}, 联系人: {reader["contact_person"]}");
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine($"   ✗ 表 '{table}' 不存在或查询失败: {ex.Message}");
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ✗ 数据查询测试失败: {ex.Message}");
            }
            Console.WriteLine();
        }
    }
}
