using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using FurnitureManagement.Client.Models;
using FurnitureManagement.Client.Servcie;
using FurnitureManagement.Client.Services;
using FurnitureManagement.Client.Views;

namespace FurnitureManagement.Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ApiService _apiService;
        private List<User>? _allUsers;

        public MainWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
            InitializeUserInfo();
            ApplyRoleBasedPermissions();
            // 初始化时只显示系统概览，不加载用户列表
            ShowSystemOverview();
        }

        // 初始化用户信息
        private void InitializeUserInfo()
        {
            if (UserSession.CurrentUser != null)
            {
                txtUserName.Text = UserSession.CurrentUser.RealName;
                txtUserRole.Text = UserSession.CurrentUser.Role;
            }
        }

        // 应用基于角色的权限控制
        private void ApplyRoleBasedPermissions()
        {
            if (!UserSession.IsLoggedIn)
            {
                return;
            }

            // 根据角色控制功能访问权限
            // Admin - 拥有所有权限
            // Manager - 拥有大部分管理权限，但不能管理用户
            // User - 只能查看基本信息，不能进行管理操作

            if (UserSession.IsUser)
            {
                // 普通用户：隐藏所有管理功能，只能查看系统概览
                btnInventoryManage.Visibility = Visibility.Collapsed;
                btnPurchaseManage.Visibility = Visibility.Collapsed;
                btnSalesManage.Visibility = Visibility.Collapsed;
                btnFinanceManage.Visibility = Visibility.Collapsed;
                btnReportManage.Visibility = Visibility.Collapsed;
                btnProductManage.Visibility = Visibility.Collapsed;
                btnCategoryManage.Visibility = Visibility.Collapsed;
                btnSupplierManage.Visibility = Visibility.Collapsed;
                btnWarehouseManage.Visibility = Visibility.Collapsed;
                btnUserManage.Visibility = Visibility.Collapsed;
            }
            else if (UserSession.IsManager)
            {
                // 经理：有管理权限，但不能管理用户和财务
                btnUserManage.Visibility = Visibility.Collapsed;
                btnFinanceManage.Visibility = Visibility.Collapsed;
            }
            // Admin 拥有所有权限，不需要隐藏任何功能
        }

        // 显示系统概览
        private void ShowSystemOverview()
        {
            // 重置所有导航按钮样式
            ResetNavButtonStyles();
            // 设置系统概览按钮为激活状态
            SetActiveNavButton(btnSystemOverview);

            // 隐藏所有内容页面
            HideAllContentGrids();
            // 显示系统概览页面
            SystemOverviewGrid.Visibility = Visibility.Visible;
        }

        // 显示用户管理
        private void ShowUserManagement()
        {
            // 检查权限：只有管理员可以管理用户
            if (!UserSession.IsAdmin)
            {
                MessageBox.Show("您没有权限访问此功能，仅管理员可以管理用户。", "权限不足", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 重置所有导航按钮样式
            ResetNavButtonStyles();
            // 设置用户管理按钮为激活状态
            SetActiveNavButton(btnUserManage);

            // 隐藏所有内容页面
            HideAllContentGrids();
            // 显示用户管理页面
            UserManageGrid.Visibility = Visibility.Visible;

            // 加载用户列表
            LoadUsersAsync();
        }

        // 重置所有导航按钮样式
        private void ResetNavButtonStyles()
        {
            // 创建默认背景色
            var defaultBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34495E"));

            // 设置所有按钮的背景为默认值
            btnSystemOverview.Background = defaultBackground;
            btnInventoryManage.Background = defaultBackground;
            btnPurchaseManage.Background = defaultBackground;
            btnSalesManage.Background = defaultBackground;
            btnFinanceManage.Background = defaultBackground;
            btnReportManage.Background = defaultBackground;
            btnProductManage.Background = defaultBackground;
            btnCategoryManage.Background = defaultBackground;
            btnSupplierManage.Background = defaultBackground;
            btnWarehouseManage.Background = defaultBackground;
            btnUserManage.Background = defaultBackground;
        }

        // 加载用户列表
        private async void LoadUsersAsync()
        {
            try
            {
                _allUsers = await _apiService.GetUsersAsync();
                dgUsers.ItemsSource = _allUsers;
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载用户列表失败，请重试", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"加载用户失败: {ex.Message}");
            }
        }

        // 退出登录
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            // 显示确认对话框
            var result = MessageBox.Show("确定要退出登录吗？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                // 清除登录信息，防止自动登录
                using (var store = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    string loginInfoFile = "logininfo.txt";
                    if (store.FileExists(loginInfoFile))
                    {
                        store.DeleteFile(loginInfoFile);
                    }
                }
                
                // 关闭当前窗口，打开登录窗口
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        // 系统概览按钮点击事件
        private void btnSystemOverview_Click(object sender, RoutedEventArgs e)
        {
            ShowSystemOverview();
        }

        // 库存管理按钮点击事件
        private void btnInventoryManage_Click(object sender, RoutedEventArgs e)
        {
            // 重置所有导航按钮样式
            ResetNavButtonStyles();
            // 设置当前按钮为激活状态
            SetActiveNavButton(btnInventoryManage);

            // 隐藏所有内容页面
            HideAllContentGrids();
            // 显示库存管理页面
            InventoryManageFrame.Navigate(new InventoryManagementPage());
            InventoryManageFrame.Visibility = Visibility.Visible;
        }

        // 进货管理按钮点击事件
        private void btnPurchaseManage_Click(object sender, RoutedEventArgs e)
        {
            // 重置所有导航按钮样式
            ResetNavButtonStyles();
            // 设置当前按钮为激活状态
            SetActiveNavButton(btnPurchaseManage);

            // 隐藏所有内容页面
            HideAllContentGrids();
            // 显示采购管理页面
            PurchaseManageFrame.Navigate(new PurchaseManagementPage());
            PurchaseManageFrame.Visibility = Visibility.Visible;
        }

        // 销售管理按钮点击事件
        private void btnSalesManage_Click(object sender, RoutedEventArgs e)
        {
            // 重置所有导航按钮样式
            ResetNavButtonStyles();
            // 设置当前按钮为激活状态
            SetActiveNavButton(btnSalesManage);

            // 隐藏所有内容页面
            HideAllContentGrids();
            // 显示销售管理页面，传递当前用户ID
            SalesManageFrame.Navigate(new SaleOrderManagementPage(UserSession.CurrentUser?.UserId ?? 0));
            SalesManageFrame.Visibility = Visibility.Visible;
        }

        // 资金管理按钮点击事件
        private void btnFinanceManage_Click(object sender, RoutedEventArgs e)
        {
            // 重置所有导航按钮样式
            ResetNavButtonStyles();
            // 设置当前按钮为激活状态
            SetActiveNavButton(btnFinanceManage);

            // 隐藏所有内容页面
            HideAllContentGrids();
            // 显示资金管理页面
            FinanceManageFrame.Navigate(new CapitalFlowManagementPage());
            FinanceManageFrame.Visibility = Visibility.Visible;
        }

        // 统计报表按钮点击事件
        private void btnReportManage_Click(object sender, RoutedEventArgs e)
        {
            // 重置所有导航按钮样式
            ResetNavButtonStyles();
            // 设置当前按钮为激活状态
            SetActiveNavButton(btnReportManage);

            // 隐藏所有内容页面
            HideAllContentGrids();
            // 显示统计报表页面
            ReportManageFrame.Navigate(new ReportManagementPage());
            ReportManageFrame.Visibility = Visibility.Visible;
        }

        // 商品管理按钮点击事件
        private void btnProductManage_Click(object sender, RoutedEventArgs e)
        {
            // 重置所有导航按钮样式
            ResetNavButtonStyles();
            // 设置当前按钮为激活状态
            SetActiveNavButton(btnProductManage);

            // 隐藏所有内容页面
            HideAllContentGrids();
            // 显示商品管理页面，传入当前用户ID
            ProductManageFrame.Navigate(new ProductManagementPage(UserSession.CurrentUser?.UserId ?? 0));
            ProductManageFrame.Visibility = Visibility.Visible;
        }

        // 分类管理按钮点击事件
        private void btnCategoryManage_Click(object sender, RoutedEventArgs e)
        {
            // 重置所有导航按钮样式
            ResetNavButtonStyles();
            // 设置当前按钮为激活状态
            SetActiveNavButton(btnCategoryManage);
            
            // 隐藏所有内容页面
            HideAllContentGrids();
            // 显示分类管理页面
            CategoryManageFrame.Navigate(new CategoryManagementPage());
            CategoryManageFrame.Visibility = Visibility.Visible;
        }

        // 供应商管理按钮点击事件
        private void btnSupplierManage_Click(object sender, RoutedEventArgs e)
        {
            // 重置所有导航按钮样式
            ResetNavButtonStyles();
            // 设置当前按钮为激活状态
            SetActiveNavButton(btnSupplierManage);

            // 隐藏所有内容页面
            HideAllContentGrids();
            // 显示供应商管理页面
            SupplierManageFrame.Navigate(new SupplierManagementPage());
            SupplierManageFrame.Visibility = Visibility.Visible;
        }

        // 仓库管理按钮点击事件
        private void btnWarehouseManage_Click(object sender, RoutedEventArgs e)
        {
            // 重置所有导航按钮样式
            ResetNavButtonStyles();
            // 设置当前按钮为激活状态
            SetActiveNavButton(btnWarehouseManage);

            // 隐藏所有内容页面
            HideAllContentGrids();
            // 显示仓库管理页面
            WarehouseManageFrame.Navigate(new WarehouseManagementPage());
            WarehouseManageFrame.Visibility = Visibility.Visible;
        }

        // 隐藏所有内容网格
        private void HideAllContentGrids()
        {
            SystemOverviewGrid.Visibility = Visibility.Collapsed;
            InventoryManageFrame.Visibility = Visibility.Collapsed;
            PurchaseManageFrame.Visibility = Visibility.Collapsed;
            SalesManageFrame.Visibility = Visibility.Collapsed;
            FinanceManageFrame.Visibility = Visibility.Collapsed;
            ReportManageFrame.Visibility = Visibility.Collapsed;
            ProductManageFrame.Visibility = Visibility.Collapsed;
            CategoryManageFrame.Visibility = Visibility.Collapsed;
            SupplierManageFrame.Visibility = Visibility.Collapsed;
            WarehouseManageFrame.Visibility = Visibility.Collapsed;
            UserManageGrid.Visibility = Visibility.Collapsed;
        }

        // 用户管理按钮点击事件
        private void btnUserManage_Click(object sender, RoutedEventArgs e)
        {
            ShowUserManagement();
        }

        // 添加用户按钮点击事件
        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            // 检查权限
            if (!UserSession.IsAdmin)
            {
                MessageBox.Show("您没有权限添加用户。", "权限不足", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 打开用户编辑窗口（新建用户）
            var editWindow = new UserEditWindow(null);
            if (editWindow.ShowDialog() == true)
            {
                // 刷新用户列表
                LoadUsersAsync();
            }
        }

        // 搜索按钮点击事件
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (_allUsers == null)
                return;

            string searchText = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                dgUsers.ItemsSource = _allUsers;
            }
            else
            {
                var filteredUsers = _allUsers.Where(u =>
                    u.Username.ToLower().Contains(searchText) ||
                    u.Email?.ToLower().Contains(searchText) == true ||
                    u.RealName.ToLower().Contains(searchText)
                ).ToList();
                dgUsers.ItemsSource = filteredUsers;
            }
        }

        // 编辑用户按钮点击事件
        private void btnEditUser_Click(object sender, RoutedEventArgs e)
        {
            // 检查权限
            if (!UserSession.IsAdmin)
            {
                MessageBox.Show("您没有权限编辑用户。", "权限不足", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 获取选中的用户
            var button = sender as Button;
            var user = button?.Tag as User;
            if (user != null)
            {
                // 打开用户编辑窗口
                var editWindow = new UserEditWindow(user);
                if (editWindow.ShowDialog() == true)
                {
                    // 刷新用户列表
                    LoadUsersAsync();
                }
            }
        }

        // 删除用户按钮点击事件
        private async void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            // 检查权限
            if (!UserSession.IsAdmin)
            {
                MessageBox.Show("您没有权限删除用户。", "权限不足", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var button = sender as Button;
            var user = button?.Tag as User;
            if (user != null)
            {
                // 防止删除自己
                if (user.UserId == UserSession.CurrentUser?.UserId)
                {
                    MessageBox.Show("不能删除当前登录的用户。", "操作失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 显示确认对话框
                var result = MessageBox.Show($"确定要删除用户 {user.Username} 吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var response = await _apiService.DeleteUserAsync(user.UserId);
                        if (response != null && response.Success)
                        {
                            MessageBox.Show("用户删除成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadUsersAsync();
                        }
                        else
                        {
                            MessageBox.Show(response?.Message ?? "删除失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("删除用户失败，请重试", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        Console.WriteLine($"删除用户异常: {ex.Message}");
                    }
                }
            }
        }

        // 切换用户状态按钮点击事件
        private async void btnToggleStatus_Click(object sender, RoutedEventArgs e)
        {
            // 检查权限
            if (!UserSession.IsAdmin)
            {
                MessageBox.Show("您没有权限修改用户状态。", "权限不足", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var button = sender as Button;
            var user = button?.Tag as User;
            if (user != null)
            {
                // 防止禁用自己
                if (user.UserId == UserSession.CurrentUser?.UserId)
                {
                    MessageBox.Show("不能修改当前登录用户的状态。", "操作失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                try
                {
                    var response = user.Status == "active"
                        ? await _apiService.DeactivateUserAsync(user.UserId)
                        : await _apiService.ActivateUserAsync(user.UserId);

                    if (response != null && response.Success)
                    {
                        MessageBox.Show("用户状态更新成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadUsersAsync();
                    }
                    else
                    {
                        MessageBox.Show(response?.Message ?? "状态更新失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("更新用户状态失败，请重试", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine($"更新用户状态异常: {ex.Message}");
                }
            }
        }
        
        // 设置激活的导航按钮样式
        private void SetActiveNavButton(Button button)
        {
            if (button != null)
            {
                button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2C3E50"));
            }
        }

        // 修改密码按钮点击事件
        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var changePasswordWindow = new ChangePasswordWindow();
            changePasswordWindow.ShowDialog();
        }
    }

    // 状态转换器，用于将状态转换为颜色
    public class StatusToColorConverter : IValueConverter
    {
        public static StatusToColorConverter Instance { get; } = new StatusToColorConverter();
        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string status)
            {
                return status == "active" ? Brushes.Green : Brushes.Red;
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}