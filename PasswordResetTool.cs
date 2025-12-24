using System;
using System.Security.Cryptography;
using System.Text;

namespace FurnitureManagement.PasswordResetTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== 家具管理系统密码重置工具 ===");
            Console.WriteLine();
            
            // 验证您提供的哈希值
            string yourHash = "fcea920f7412b5da7be0cf42b8c93759";
            Console.WriteLine($"您提供的哈希值: {yourHash}");
            
            // 尝试一些常见密码
            string[] commonPasswords = {
                "123456", "admin", "password", "123123", "111111", 
                "000000", "12345678", "admin123", "root", "test",
                "user", "manager", "system", "demo", "guest"
            };
            
            Console.WriteLine("\n正在尝试常见密码...");
            foreach (string password in commonPasswords)
            {
                string hash = EncryptPassword(password);
                if (hash.Equals(yourHash, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"✅ 找到密码: {password}");
                    Console.WriteLine($"   对应哈希: {hash}");
                    return;
                }
            }
            
            Console.WriteLine("❌ 未找到匹配的常见密码");
            Console.WriteLine();
            
            // 生成新密码的哈希值
            Console.WriteLine("=== 生成新密码哈希值 ===");
            Console.Write("请输入新密码: ");
            string newPassword = Console.ReadLine() ?? "";
            
            if (!string.IsNullOrEmpty(newPassword))
            {
                string newHash = EncryptPassword(newPassword);
                Console.WriteLine($"新密码: {newPassword}");
                Console.WriteLine($"新哈希: {newHash}");
                Console.WriteLine();
                Console.WriteLine("请将此哈希值更新到数据库的user表中对应用户的password字段");
            }
            
            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }
        
        /// <summary>
        /// 使用MD5加密密码（与系统中的PasswordHelper.EncryptPassword方法相同）
        /// </summary>
        public static string EncryptPassword(string password)
        {
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
