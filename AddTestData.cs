using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace AddTestData
{
    class Program
    {
        static void Main(string[] args)
        {
            // 数据库连接字符串
            string connectionString = "server=localhost;user=root;password=123456;database=furniture_management;";

            // 创建连接
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("成功连接到数据库");

                    // 插入测试数据
                    InsertTestData(connection);

                    // 查询插入的数据
                    QueryTestData(connection);

                    Console.WriteLine("测试数据插入完成");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"操作失败: {ex.Message}");
                }
            }

            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
        }

        /// <summary>
        /// 插入测试数据到capital_flow表
        /// </summary>
        static void InsertTestData(MySqlConnection connection)
        {
            // SQL插入语句
            string insertSql = @"INSERT INTO capital_flow (flow_date, flow_type, amount, description, reference_type, reference_id, created_by, created_at)
VALUES
    -- 收入记录
    ('2025-12-20', 'income', 3800.00, '销售沙发', 'sale', 1, 1, NOW()),
    ('2025-12-21', 'income', 7400.00, '销售沙发和餐桌', 'sale', 2, 1, NOW()),
    ('2025-12-22', 'income', 3800.00, '销售沙发', 'sale', 3, 1, NOW()),
    ('2025-12-23', 'income', 5800.00, '销售沙发和餐桌', 'sale', 4, 1, NOW()),
    ('2025-12-24', 'income', 14200.00, '批量销售沙发', 'sale', 5, 1, NOW()),
    
    -- 支出记录
    ('2025-12-15', 'expense', 2500.00, '采购沙发', 'purchase', 1, 1, NOW()),
    ('2025-12-16', 'expense', 1200.00, '采购餐桌', 'purchase', 2, 1, NOW()),
    ('2025-12-17', 'expense', 500.00, '采购配件', 'purchase', 3, 1, NOW()),
    ('2025-12-18', 'expense', 800.00, '仓库租金', 'other', NULL, 1, NOW()),
    ('2025-12-19', 'expense', 300.00, '水电费', 'other', NULL, 1, NOW());";

            using (MySqlCommand command = new MySqlCommand(insertSql, connection))
            {
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine($"成功插入 {rowsAffected} 条记录");
            }
        }

        /// <summary>
        /// 查询capital_flow表中的数据
        /// </summary>
        static void QueryTestData(MySqlConnection connection)
        {
            string querySql = "SELECT * FROM capital_flow";

            using (MySqlCommand command = new MySqlCommand(querySql, connection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine("\ncapital_flow表中的数据:");
                    Console.WriteLine("---------------------------------------------------------------------------------------------------");
                    Console.WriteLine("FlowId | FlowDate   | FlowType | Amount   | Description     | ReferenceType | ReferenceId | CreatedBy | CreatedAt");
                    Console.WriteLine("---------------------------------------------------------------------------------------------------");

                    while (reader.Read())
                    {
                        int flowId = reader.GetInt32("flow_id");
                        DateTime flowDate = reader.GetDateTime("flow_date");
                        string flowType = reader.GetString("flow_type");
                        decimal amount = reader.GetDecimal("amount");
                        string description = reader.GetString("description");
                        string referenceType = reader.GetString("reference_type");
                        int? referenceId = reader.IsDBNull(reader.GetOrdinal("reference_id")) ? (int?)null : reader.GetInt32("reference_id");
                        int? createdBy = reader.IsDBNull(reader.GetOrdinal("created_by")) ? (int?)null : reader.GetInt32("created_by");
                        DateTime createdAt = reader.GetDateTime("created_at");

                        Console.WriteLine($"{flowId,-7} | {flowDate:yyyy-MM-dd} | {flowType,-8} | {amount,8:C2} | {description,-15} | {referenceType,-14} | {referenceId,-11} | {createdBy,-9} | {createdAt:yyyy-MM-dd HH:mm:ss}");
                    }

                    Console.WriteLine("---------------------------------------------------------------------------------------------------");
                }
            }
        }
    }
}
