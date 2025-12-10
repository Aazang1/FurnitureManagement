using System.Windows;
using System.Text.RegularExpressions;
using FurnitureManagement.Client.Models;
using FurnitureManagement.Client.Servcie;

namespace FurnitureManagement.Client.Views
{
    /// <summary>
    /// RegisterWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private readonly ApiService _apiService;

        public RegisterWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        // 注册按钮点击事件
        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            // 验证输入
            if (!ValidateInput())
                return;

            btnRegister.IsEnabled = false;
            txtError.Text = "";

            try
            {
                var request = new RegisterRequest
                {
                    Username = txtUsername.Text.Trim(),
                    Password = txtPassword.Password,
                    Email = txtEmail.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Role = "User" // 默认注册为普通用户
                };

                var response = await _apiService.RegisterAsync(request);

                if (response != null && response.Success)
                {
                    MessageBox.Show("注册成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    txtError.Text = response?.Message ?? "注册失败";
                }
            }
            catch (Exception ex)
            {
                txtError.Text = "注册时发生错误，请重试";
                Console.WriteLine($"注册异常: {ex.Message}");
            }
            finally
            {
                btnRegister.IsEnabled = true;
            }
        }

        // 取消按钮点击事件
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // 验证输入
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                txtError.Text = "请输入用户名";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                txtError.Text = "请输入密码";
                return false;
            }

            if (txtPassword.Password.Length < 6)
            {
                txtError.Text = "密码长度不能少于6位";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                txtError.Text = "请输入邮箱";
                return false;
            }

            // 简单的邮箱格式验证
            if (!Regex.IsMatch(txtEmail.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                txtError.Text = "请输入有效的邮箱地址";
                return false;
            }

            return true;
        }
    }
}