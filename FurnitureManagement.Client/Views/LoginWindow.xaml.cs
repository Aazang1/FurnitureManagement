using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
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
        private const string LoginInfoFile = "logininfo.txt";

        public LoginWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
            LoadLoginInfo();
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
                        // 登录成功，保存登录信息
                        SaveLoginInfo();
                        
                        // 设置用户会话
                        UserSession.CurrentUser = response.User;
                        
                        // 打开主窗口
                        Console.WriteLine("登录成功，准备打开主窗口...");
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

        // 加载登录信息
        private void LoadLoginInfo()
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    if (store.FileExists(LoginInfoFile))
                    {
                        using (var stream = new IsolatedStorageFileStream(LoginInfoFile, FileMode.Open, store))
                        using (var reader = new StreamReader(stream))
                        {
                            string line = reader.ReadLine();
                            if (!string.IsNullOrEmpty(line))
                            {
                                string[] parts = line.Split('|');
                                if (parts.Length >= 4)
                                {
                                    txtUsername.Text = parts[0];
                                    txtPassword.Password = DecodeBase64(parts[1]);
                                    chkRememberPassword.IsChecked = bool.Parse(parts[2]);
                                    chkAutoLogin.IsChecked = bool.Parse(parts[3]);

                                    // 如果自动登录选项为true，则自动执行登录
                                    if (chkAutoLogin.IsChecked == true)
                                    {
                                        btnLogin_Click(null, null);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载登录信息失败: {ex.Message}");
            }
        }

        // 保存登录信息
        private void SaveLoginInfo()
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    using (var stream = new IsolatedStorageFileStream(LoginInfoFile, FileMode.Create, store))
                    using (var writer = new StreamWriter(stream))
                    {
                        string username = txtUsername.Text.Trim();
                        string password = chkRememberPassword.IsChecked == true ? EncodeBase64(txtPassword.Password) : string.Empty;
                        bool rememberPassword = chkRememberPassword.IsChecked == true;
                        bool autoLogin = chkAutoLogin.IsChecked == true;

                        writer.WriteLine($"{username}|{password}|{rememberPassword}|{autoLogin}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存登录信息失败: {ex.Message}");
            }
        }

        // Base64编码
        private string EncodeBase64(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes);
        }

        // Base64解码
        private string DecodeBase64(string input)
        {
            byte[] bytes = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}