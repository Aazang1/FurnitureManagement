using System;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Data;

namespace FurnitureManagement.TestSupplierDatabase
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== 供应商数据库测试工具 ===");
            Console.WriteLine();
            
            string connectionString = "server=localhost;database=furniture_management;user=root;password=Y2486yzl;port=3306";
            
            try
            {
                using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();
                Console.WriteLine("✓ 数据库连接成功");
                
                // 1. 检查表结构
                await CheckTableStructure(connection);
                
                // 2. 查询数据
                await QuerySupplierData(connection);
                
                // 3. 测试插入数据
                await TestInsertData(connection);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ 数据库操作失败: {ex.Message}");
                Console.WriteLine($"详细错误: {ex}");
            }
            
            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }
        
        static async Task CheckTableStructure(MySqlConnection connection)
        {
            Console.WriteLine("\n=== 检查表结构 ===");
            
            try
            {
                string sql = @"
                    SELECT 
                        COLUMN_NAME,
                        DATA_TYPE,
                        IS_NULLABLE,
                        COLUMN_DEFAULT,
                        COLUMN_KEY
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_SCHEMA = 'furniture_management' 
                    AND TABLE_NAME = 'supplier'
                    ORDER BY ORDINAL_POSITION";
                
                using var command = new MySqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();
                
                Console.WriteLine("字段名\t\t数据类型\t可空\t默认值\t\t键");
                Console.WriteLine("".PadRight(80, '-'));
                
                while (await reader.ReadAsync())
                {
                    string columnName = reader["COLUMN_NAME"].ToString();
                    string dataType = reader["DATA_TYPE"].ToString();
                    string isNullable = reader["IS_NULLABLE"].ToString();
                    string columnDefault = reader["COLUMN_DEFAULT"]?.ToString() ?? "NULL";
                    string columnKey = reader["COLUMN_KEY"]?.ToString() ?? "";
                    
                    Console.WriteLine($"{columnName.PadRight(15)}\t{dataType.PadRight(10)}\t{isNullable}\t{columnDefault.PadRight(15)}\t{columnKey}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ 检查表结构失败: {ex.Message}");
            }
        }
        
        static async Task QuerySupplierData(MySqlConnection connection)
        {
            Console.WriteLine("\n=== 查询供应商数据 ===");
            
            try
            {
                string sql = "SELECT * FROM supplier LIMIT 5";
                using var command = new MySqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();
                
                // 获取列信息
                var columnCount = reader.FieldCount;
                var columnNames = new string[columnCount];
                for (int i = 0; i < columnCount; i++)
                {
                    columnNames[i] = reader.GetName(i);
                }
                
                // 显示列标题
                Console.WriteLine(string.Join("\t", columnNames));
                Console.WriteLine("".PadRight(100, '-'));
                
                // 显示数据
                while (await reader.ReadAsync())
                {
                    var values = new string[columnCount];
                    for (int i = 0; i < columnCount; i++)
                    {
                        values[i] = reader.IsDBNull(i) ? "NULL" : reader[i].ToString();
                    }
                    Console.WriteLine(string.Join("\t", values));
                }
                
                reader.Close();
                
                // 统计总数
                command.CommandText = "SELECT COUNT(*) FROM supplier";
                var count = await command.ExecuteScalarAsync();
                Console.WriteLine($"\n总供应商数量: {count}");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ 查询数据失败: {ex.Message}");
            }
        }
        
        static async Task TestInsertData(MySqlConnection connection)
        {
            Console.WriteLine("\n=== 测试插入数据 ===");
            
            try
            {
                // 检查是否已存在测试数据
                string checkSql = "SELECT COUNT(*) FROM supplier WHERE supplier_name = '测试供应商API'";
                using var checkCommand = new MySqlCommand(checkSql, connection);
                var existingCount = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());
                
                if (existingCount > 0)
                {
                    Console.WriteLine("测试数据已存在，跳过插入");
                    return;
                }
                
                // 插入测试数据
                string insertSql = @"
                    INSERT INTO supplier (supplier_name, contact_person, phone, email, address, created_at, updated_at) 
                    VALUES (@name, @contact, @phone, @email, @address, @created, @updated)";
                
                using var insertCommand = new MySqlCommand(insertSql, connection);
                insertCommand.Parameters.AddWithValue("@name", "测试供应商API");
                insertCommand.Parameters.AddWithValue("@contact", "测试联系人");
                insertCommand.Parameters.AddWithValue("@phone", "13800000000");
                insertCommand.Parameters.AddWithValue("@email", "test@api.com");
                insertCommand.Parameters.AddWithValue("@address", "测试地址");
                insertCommand.Parameters.AddWithValue("@created", DateTime.Now);
                insertCommand.Parameters.AddWithValue("@updated", DateTime.Now);
                
                int rowsAffected = await insertCommand.ExecuteNonQueryAsync();
                Console.WriteLine($"✓ 插入成功，影响行数: {rowsAffected}");
                
                // 查询刚插入的数据
                string selectSql = "SELECT * FROM supplier WHERE supplier_name = '测试供应商API'";
                using var selectCommand = new MySqlCommand(selectSql, connection);
                using var reader = await selectCommand.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    Console.WriteLine("插入的数据:");
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string value = reader.IsDBNull(i) ? "NULL" : reader[i].ToString();
                        Console.WriteLine($"  {reader.GetName(i)}: {value}");
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ 插入测试失败: {ex.Message}");
                Console.WriteLine($"详细错误: {ex}");
            }
        }
    }
}
