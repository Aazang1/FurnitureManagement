using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FurnitureManagement.TestSupplierAPI
{
    class Program
    {
        private static readonly HttpClient httpClient = new HttpClient();
        
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== 供应商API测试工具 ===");
            Console.WriteLine();
            
            // 设置API基础地址
            string baseUrl = "http://localhost:5192/api";
            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.Timeout = TimeSpan.FromSeconds(30);
            
            Console.WriteLine($"测试API地址: {baseUrl}");
            Console.WriteLine();
            
            // 测试供应商API
            await TestGetSuppliers();
            await TestCreateSupplier();
            
            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }
        
        static async Task TestGetSuppliers()
        {
            Console.WriteLine("=== 测试获取供应商列表 ===");
            try
            {
                var response = await httpClient.GetAsync("/Supplier");
                Console.WriteLine($"HTTP状态码: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"响应内容长度: {content.Length} 字符");
                    Console.WriteLine($"响应内容: {content}");
                    
                    try
                    {
                        var suppliers = await response.Content.ReadFromJsonAsync<List<Supplier>>();
                        if (suppliers != null)
                        {
                            Console.WriteLine($"✓ 成功解析JSON，供应商数量: {suppliers.Count}");
                            foreach (var supplier in suppliers)
                            {
                                Console.WriteLine($"  - ID: {supplier.SupplierId}, 名称: {supplier.SupplierName}");
                                Console.WriteLine($"    联系人: {supplier.ContactPerson ?? "无"}");
                                Console.WriteLine($"    电话: {supplier.Phone ?? "无"}");
                                Console.WriteLine($"    邮箱: {supplier.Email ?? "无"}");
                                Console.WriteLine($"    地址: {supplier.Address ?? "无"}");
                                Console.WriteLine();
                            }
                        }
                        else
                        {
                            Console.WriteLine("✗ JSON解析返回null");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"✗ JSON解析失败: {ex.Message}");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"✗ 请求失败，错误内容: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ 请求异常: {ex.Message}");
                Console.WriteLine($"详细错误: {ex}");
            }
            Console.WriteLine();
        }
        
        static async Task TestCreateSupplier()
        {
            Console.WriteLine("=== 测试创建供应商 ===");
            try
            {
                var newSupplier = new Supplier
                {
                    SupplierName = "测试供应商",
                    ContactPerson = "测试联系人",
                    Phone = "13800000000",
                    Email = "test@test.com",
                    Address = "测试地址",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                
                var response = await httpClient.PostAsJsonAsync("/Supplier", newSupplier);
                Console.WriteLine($"HTTP状态码: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"✓ 创建成功，响应: {content}");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"✗ 创建失败，错误: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ 创建异常: {ex.Message}");
            }
            Console.WriteLine();
        }
    }
    
    // 简化的供应商模型用于测试
    public class Supplier
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
