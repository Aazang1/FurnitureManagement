using System.Windows;
using FurnitureManagement.Client.Models;
using FurnitureManagement.Client.Servcie;
using FurnitureManagement.Client.Services;

namespace FurnitureManagement.Client.Views
{
    /// <summary>
    /// ChangePasswordWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        private readonly ApiService _apiService;

        public ChangePasswordWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        // 确认修改按钮点击事件
        private async void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            // 验证输入
            if (string.IsNullOrWhiteSpace(txtOldPassword.Password))
            {
                txtError.Text = "请输入原密码";
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNewPassword.Password))
            {
                txtError.Text = "请输入新密码";
                return;
            }

            if (string.IsNullOrWhiteSpace(txtConfirmPassword.Password))
            {
                txtError.Text = "请确认新密码";
                return;
            }

            if (txtNewPassword.Password != txtConfirmPassword.Password)
            {
                txtError.Text = "两次输入的新密码不一致";
                return;
            }

            if (txtNewPassword.Password.Length < 6)
            {
                txtError.Text = "新密码长度不能少于6位";
                return;
            }

            if (txtOldPassword.Password == txtNewPassword.Password)
            {
                txtError.Text = "新密码不能与原密码相同";
                return;
            }

            // 检查用户是否已登录
            if (!UserSession.IsLoggedIn || UserSession.CurrentUser == null)
            {
                txtError.Text = "用户未登录";
                return;
            }

            btnConfirm.IsEnabled = false;
            txtError.Text = "正在修改密码...";
            txtError.Foreground = System.Windows.Media.Brushes.Blue;

            try
            {
                var request = new ChangePasswordRequest
                {
                    UserId = UserSession.CurrentUser.UserId,
                    OldPassword = txtOldPassword.Password,
                    NewPassword = txtNewPassword.Password
                };

                var response = await _apiService.ChangePasswordAsync(request);

                if (response != null && response.Success)
                {
                    MessageBox.Show("密码修改成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    txtError.Text = response?.Message ?? "修改失败";
                    txtError.Foreground = System.Windows.Media.Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                txtError.Text = $"修改密码时发生错误: {ex.Message}";
                txtError.Foreground = System.Windows.Media.Brushes.Red;
                Console.WriteLine($"修改密码异常: {ex.Message}");
            }
            finally
            {
                btnConfirm.IsEnabled = true;
            }
        }

        // 取消按钮点击事件
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
