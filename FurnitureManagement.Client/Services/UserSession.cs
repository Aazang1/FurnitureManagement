using FurnitureManagement.Client.Models;

namespace FurnitureManagement.Client.Services
{
    /// <summary>
    /// 用户会话管理类，用于存储当前登录用户信息
    /// </summary>
    public static class UserSession
    {
        private static User? _currentUser;

        /// <summary>
        /// 获取或设置当前登录用户
        /// </summary>
        public static User? CurrentUser
        {
            get => _currentUser;
            set => _currentUser = value;
        }

        /// <summary>
        /// 检查用户是否已登录
        /// </summary>
        public static bool IsLoggedIn => _currentUser != null;

        /// <summary>
        /// 检查当前用户是否为管理员
        /// </summary>
        public static bool IsAdmin => _currentUser?.Role == "Admin";

        /// <summary>
        /// 检查当前用户是否为经理
        /// </summary>
        public static bool IsManager => _currentUser?.Role == "Manager";

        /// <summary>
        /// 检查当前用户是否为普通用户
        /// </summary>
        public static bool IsUser => _currentUser?.Role == "User";

        /// <summary>
        /// 检查当前用户是否有指定角色
        /// </summary>
        /// <param name="role">角色名称</param>
        /// <returns>是否拥有该角色</returns>
        public static bool HasRole(string role)
        {
            return _currentUser?.Role == role;
        }

        /// <summary>
        /// 检查当前用户是否有权限（Admin和Manager都有管理权限）
        /// </summary>
        public static bool HasManagementPermission => IsAdmin || IsManager;

        /// <summary>
        /// 清除会话信息（退出登录时调用）
        /// </summary>
        public static void Clear()
        {
            _currentUser = null;
        }
    }
}
