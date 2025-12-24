using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace FurnitureManagement.DiagnosticTool
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== 家具管理系统诊断工具 ===");
            Console.WriteLine();
            
            // 1. 检查.NET环境
            CheckDotNetEnvironment();
            
            // 2. 检查网络连接
            await CheckNetworkConnectivity();
            
            // 3. 检查端口可用性
            CheckPortAvailability();
            
            // 4. 尝试启动简单的HTTP服务器
            await TestSimpleHttpServer();
            
            Console.WriteLine("\n=== 诊断完成 ===");
            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
        }
        
        static void CheckDotNetEnvironment()
        {
            Console.WriteLine("1. 检查.NET环境:");
            try
            {
                var version = Environment.Version;
                Console.WriteLine($"   ✓ .NET版本: {version}");
                Console.WriteLine($"   ✓ 操作系统: {Environment.OSVersion}");
                Console.WriteLine($"   ✓ 64位系统: {Environment.Is64BitOperatingSystem}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ✗ .NET环境检查失败: {ex.Message}");
            }
            Console.WriteLine();
        }
        
        static async Task CheckNetworkConnectivity()
        {
            Console.WriteLine("2. 检查网络连接:");
            
            // 检查本地回环
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync("127.0.0.1", 1000);
                if (reply.Status == IPStatus.Success)
                {
                    Console.WriteLine("   ✓ 本地回环连接正常");
                }
                else
                {
                    Console.WriteLine($"   ✗ 本地回环连接失败: {reply.Status}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ✗ 网络连接检查失败: {ex.Message}");
            }
            
            // 检查HTTP连接
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(5);
                var response = await client.GetAsync("http://www.baidu.com");
                Console.WriteLine($"   ✓ 外网HTTP连接正常 (状态码: {response.StatusCode})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ✗ 外网HTTP连接失败: {ex.Message}");
            }
            Console.WriteLine();
        }
        
        static void CheckPortAvailability()
        {
            Console.WriteLine("3. 检查端口可用性:");
            
            int[] ports = { 5192, 7246, 3306 }; // HTTP, HTTPS, MySQL
            
            foreach (int port in ports)
            {
                try
                {
                    var processes = Process.GetProcesses()
                        .Where(p => {
                            try 
                            {
                                return p.ProcessName.Contains("dotnet") || 
                                       p.ProcessName.Contains("mysql") ||
                                       p.ProcessName.Contains("FurnitureManagement");
                            }
                            catch { return false; }
                        });
                    
                    Console.WriteLine($"   端口 {port}: 检查中...");
                    
                    // 简单的端口检查
                    using var client = new System.Net.Sockets.TcpClient();
                    var result = client.BeginConnect("127.0.0.1", port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(1000);
                    
                    if (success && client.Connected)
                    {
                        Console.WriteLine($"   ✓ 端口 {port} 有服务在监听");
                        client.EndConnect(result);
                    }
                    else
                    {
                        Console.WriteLine($"   ○ 端口 {port} 没有服务监听");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ✗ 端口 {port} 检查失败: {ex.Message}");
                }
            }
            Console.WriteLine();
        }
        
        static async Task TestSimpleHttpServer()
        {
            Console.WriteLine("4. 测试HTTP客户端:");
            
            string[] testUrls = {
                "http://localhost:5192",
                "http://localhost:5192/api",
                "http://localhost:5192/api/Supplier",
                "https://localhost:7246",
                "https://localhost:7246/api/Supplier"
            };
            
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            
            // 忽略SSL证书错误（仅用于测试）
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            
            using var httpsClient = new HttpClient(handler);
            httpsClient.Timeout = TimeSpan.FromSeconds(10);
            
            foreach (string url in testUrls)
            {
                try
                {
                    var testClient = url.StartsWith("https") ? httpsClient : client;
                    var response = await testClient.GetAsync(url);
                    Console.WriteLine($"   ✓ {url} - 状态码: {response.StatusCode}");
                    
                    if (url.Contains("Supplier"))
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"     响应长度: {content.Length} 字符");
                        if (content.Length > 0 && content.Length < 1000)
                        {
                            Console.WriteLine($"     响应内容: {content.Substring(0, Math.Min(200, content.Length))}...");
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"   ✗ {url} - HTTP错误: {ex.Message}");
                }
                catch (TaskCanceledException ex)
                {
                    Console.WriteLine($"   ✗ {url} - 超时: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ✗ {url} - 其他错误: {ex.Message}");
                }
            }
        }
    }
}
