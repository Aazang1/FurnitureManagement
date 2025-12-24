using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureManagement.SimpleTestServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== 简单测试服务器 ===");
            Console.WriteLine("启动HTTP监听器在 http://localhost:5192/");
            Console.WriteLine("按 Ctrl+C 停止服务器");
            Console.WriteLine();
            
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5192/");
            
            try
            {
                listener.Start();
                Console.WriteLine("✓ 服务器启动成功！");
                Console.WriteLine("测试URL:");
                Console.WriteLine("  - http://localhost:5192/");
                Console.WriteLine("  - http://localhost:5192/api/supplier");
                Console.WriteLine();
                
                while (true)
                {
                    var context = await listener.GetContextAsync();
                    _ = Task.Run(() => HandleRequest(context));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ 服务器启动失败: {ex.Message}");
                Console.WriteLine();
                Console.WriteLine("可能的原因:");
                Console.WriteLine("1. 端口5192被其他程序占用");
                Console.WriteLine("2. 需要管理员权限");
                Console.WriteLine("3. 防火墙阻止了连接");
                Console.WriteLine();
                Console.WriteLine("解决方案:");
                Console.WriteLine("1. 以管理员身份运行此程序");
                Console.WriteLine("2. 检查端口占用: netstat -ano | findstr :5192");
                Console.WriteLine("3. 关闭防火墙或添加例外");
            }
            finally
            {
                listener?.Stop();
                Console.WriteLine("\n按任意键退出...");
                Console.ReadKey();
            }
        }
        
        static async Task HandleRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            
            Console.WriteLine($"收到请求: {request.HttpMethod} {request.Url}");
            
            string responseString;
            
            if (request.Url.AbsolutePath == "/")
            {
                responseString = @"
<!DOCTYPE html>
<html>
<head>
    <title>家具管理系统测试服务器</title>
    <meta charset='utf-8'>
</head>
<body>
    <h1>家具管理系统测试服务器</h1>
    <p>服务器正在运行！</p>
    <h2>测试API:</h2>
    <ul>
        <li><a href='/api/supplier'>/api/supplier</a> - 供应商API测试</li>
        <li><a href='/api/category'>/api/category</a> - 分类API测试</li>
        <li><a href='/api/furniture'>/api/furniture</a> - 家具API测试</li>
    </ul>
</body>
</html>";
                response.ContentType = "text/html; charset=utf-8";
            }
            else if (request.Url.AbsolutePath.StartsWith("/api/supplier"))
            {
                responseString = @"[
    {
        ""supplierId"": 1,
        ""supplierName"": ""华东家具厂"",
        ""contactPerson"": ""张经理"",
        ""phone"": ""13800138000"",
        ""email"": ""zhang@huadong.com"",
        ""address"": ""浙江省杭州市余杭区"",
        ""createdAt"": ""2025-10-22T08:27:40"",
        ""updatedAt"": ""2025-10-22T08:27:40""
    },
    {
        ""supplierId"": 2,
        ""supplierName"": ""君乐木业"",
        ""contactPerson"": ""李总"",
        ""phone"": ""13900139000"",
        ""email"": ""li@junle.com"",
        ""address"": ""广东省佛山市南海区"",
        ""createdAt"": ""2025-10-22T08:27:40"",
        ""updatedAt"": ""2025-10-22T08:27:40""
    },
    {
        ""supplierId"": 3,
        ""supplierName"": ""北方沙发厂"",
        ""contactPerson"": ""王总"",
        ""phone"": ""13700137000"",
        ""email"": ""wang@beifang.com"",
        ""address"": ""河北省廊坊市"",
        ""createdAt"": ""2025-10-22T08:27:40"",
        ""updatedAt"": ""2025-10-22T08:27:40""
    }
]";
                response.ContentType = "application/json; charset=utf-8";
            }
            else if (request.Url.AbsolutePath.StartsWith("/api/category"))
            {
                responseString = @"[
    {""categoryId"": 1, ""categoryName"": ""沙发""},
    {""categoryId"": 2, ""categoryName"": ""桌子""},
    {""categoryId"": 3, ""categoryName"": ""椅子""}
]";
                response.ContentType = "application/json; charset=utf-8";
            }
            else if (request.Url.AbsolutePath.StartsWith("/api/furniture"))
            {
                responseString = @"[
    {""furnitureId"": 1, ""furnitureName"": ""真皮沙发"", ""categoryId"": 1},
    {""furnitureId"": 2, ""furnitureName"": ""实木餐桌"", ""categoryId"": 2}
]";
                response.ContentType = "application/json; charset=utf-8";
            }
            else
            {
                responseString = $"{{\"error\": \"未找到路径: {request.Url.AbsolutePath}\"}}";
                response.ContentType = "application/json; charset=utf-8";
                response.StatusCode = 404;
            }
            
            // 添加CORS头
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
            
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            
            try
            {
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                response.OutputStream.Close();
                Console.WriteLine($"✓ 响应已发送: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ 发送响应失败: {ex.Message}");
            }
        }
    }
}
