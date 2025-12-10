using System.Security.Cryptography;
using System.Text;

namespace FurnitureManagement.Server.Utils
{

    public static class PasswordHelper
    {
        /// <summary>
        /// 使用MD5加密密码
        /// </summary>
        /// <param name="password">原始密码</param>
        /// <returns>MD5加密后的密码</returns>
        public static string EncryptPassword(string password)
        {
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // 将字节数组转换为十六进制字符串
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// 验证密码是否匹配
        /// </summary>
        /// <param name="password">原始密码</param>
        /// <param name="hashedPassword">加密后的密码</param>
        /// <returns>密码是否匹配</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            string hashedInput = EncryptPassword(password);
            return hashedInput.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase);
        }
    }
}
