using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FurnitureManagement.DatabaseTestTool
{
    class Program
    {
        private static readonly HttpClient httpClient = new HttpClient();
        
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== 家具管理系统数据库连接测试工具 ===");
            Console.WriteLine();
            
            // 设置API基础地址
            string baseUrl = "https://localhost:7001"; // 根据您的服务端端口调整
            httpClient.BaseAddress = new Uri(baseUrl);
            
            Console.WriteLine($"测试API地址: {baseUrl}");
            Console.WriteLine();
            
            // 测试供应商API
            await TestSuppliersAPI();
            
            // 测试其他API
            await TestCategoriesAPI();
            await TestFurnitureAPI();
            
            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }
        
        static async Task TestSuppliersAPI()
        {
            Console.WriteLine("=== 测试供应商API ===");
            try
            {
                var response = await httpClient.GetAsync("/api/Supplier");
                Console.WriteLine($"HTTP状态码: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"响应内容: {content}");
                    
                    var suppliers = await response.Content.ReadFromJsonAsync<List<Supplier>>();
                    if (suppliers != null)
                    {
                        Console.WriteLine($"供应商数量: {suppliers.Count}");
                        foreach (var supplier in suppliers)
                        {
                            Console.WriteLine($"- ID: {supplier.SupplierId}, 名称: {supplier.SupplierName}, 联系人: {supplier.ContactPerson}");
                        }
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"错误内容: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"供应商API测试失败: {ex.Message}");
                Console.WriteLine($"详细错误: {ex}");
            }
            Console.WriteLine();
        }
        
        static async Task TestCategoriesAPI()
        {
            Console.WriteLine("=== 测试分类API ===");
            try
            {
                var response = await httpClient.GetAsync("/api/Category");
                Console.WriteLine($"HTTP状态码: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"分类数据: {content}");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"错误内容: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"分类API测试失败: {ex.Message}");
            }
            Console.WriteLine();
        }
        
        static async Task TestFurnitureAPI()
        {
            Console.WriteLine("=== 测试家具API ===");
            try
            {
                var response = await httpClient.GetAsync("/api/Furniture");
                Console.WriteLine($"HTTP状态码: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"家具数据: {content}");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"错误内容: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"家具API测试失败: {ex.Message}");
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
