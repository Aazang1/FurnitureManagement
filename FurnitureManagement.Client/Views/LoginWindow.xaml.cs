using System.Windows;
using FurnitureManagement.Client.Models;
using FurnitureManagement.Client.Servcie;
using FurnitureManagement.Client.Services;

namespace FurnitureManagement.Client.Views
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly ApiService _apiService;

        public LoginWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        // 登录按钮点击事件
        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // 验证输入
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                txtError.Text = "请输入用户名和密码";
                return;
            }

            btnLogin.IsEnabled = false;
            txtError.Text = "正在登录...";

            try
            {
                var request = new LoginRequest
                {
                    Username = txtUsername.Text.Trim(),
                    Password = txtPassword.Password
                };

                Console.WriteLine("开始调用登录API...");
                var response = await _apiService.LoginAsync(request);
                Console.WriteLine($"登录API返回结果: Success={response?.Success}, Message={response?.Message}");

                if (response != null)
                {
                    if (response.Success && response.User != null)
                    {
                        // 登录成功，设置会话并打开主窗口
                        Console.WriteLine("登录成功，准备打开主窗口...");
                        UserSession.CurrentUser = response.User;
                        var mainWindow = new MainWindow();
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        txtError.Text = response.Message ?? "登录失败";
                        Console.WriteLine($"登录失败: {response.Message}");
                    }
                }
                else
                {
                    txtError.Text = "登录失败，服务器没有返回响应";
                    Console.WriteLine("登录失败，服务器没有返回响应");
                }
            }
            catch (Exception ex)
            {
                // 显示完整的错误信息，包括内部异常
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += $"\n内部错误: {ex.InnerException.Message}";
                }
                txtError.Text = $"登录时发生错误: {errorMessage}";
                
                // 输出完整的异常信息到控制台
                Console.WriteLine($"登录异常: {ex.Message}");
                Console.WriteLine($"异常堆栈: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"内部异常: {ex.InnerException.Message}");
                    Console.WriteLine($"内部异常堆栈: {ex.InnerException.StackTrace}");
                }
            }
            finally
            {
                btnLogin.IsEnabled = true;
            }
        }

        // 注册按钮点击事件
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            // 打开注册窗口
            var registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }
    }
}