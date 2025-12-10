using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using FurnitureManagement.Client.Models;
using FurnitureManagement.Client.Servcie;

namespace FurnitureManagement.Client.Views
{
    /// <summary>
    /// UserEditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UserEditWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly User? _user;
        private bool _isNewUser;

        public UserEditWindow(User? user)
        {
            InitializeComponent();
            _apiService = new ApiService();
            _user = user;
            _isNewUser = _user == null;
            InitializeWindow();
        }

        // 初始化窗口
        private void InitializeWindow()
        {
            if (_isNewUser)
            {
                txtTitle.Text = "添加用户";
                PasswordStack.Visibility = Visibility.Visible;
                cmbRole.SelectedIndex = 1; // 默认选择普通用户
                cmbStatus.SelectedIndex = 0; // 默认状态为active
            }
            else
            {
                txtTitle.Text = "编辑用户";
                PasswordStack.Visibility = Visibility.Collapsed;
                LoadUserInfo();
            }
        }

        // 加载用户信息
        private void LoadUserInfo()
        {
            if (_user != null)
            {
                txtUsername.Text = _user.Username;
                txtRealName.Text = _user.RealName;
                txtEmail.Text = _user.Email ?? "";
                txtPhone.Text = _user.Phone ?? "";
                
                // 设置角色
                foreach (ComboBoxItem item in cmbRole.Items)
                {
                    if ((string)item.Tag == _user.Role)
                    {
                        cmbRole.SelectedItem = item;
                        break;
                    }
                }
                
                // 设置状态
                foreach (ComboBoxItem item in cmbStatus.Items)
                {
                    if ((string)item.Tag == _user.Status)
                    {
                        cmbStatus.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        // 保存按钮点击事件
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // 验证输入
            if (!ValidateInput())
                return;

            btnSave.IsEnabled = false;
            txtError.Text = "";

            try
            {
                if (_isNewUser)
                {
                    // 新建用户
                    var registerRequest = new RegisterRequest
                    {
                        Username = txtUsername.Text.Trim(),
                        Password = txtPassword.Password,
                        Email = txtEmail.Text.Trim(),
                        Phone = txtPhone.Text.Trim(),
                        Role = (string)((ComboBoxItem)cmbRole.SelectedItem).Tag
                    };

                    var response = await _apiService.RegisterAsync(registerRequest);
                    if (response != null && response.Success)
                    {
                        MessageBox.Show("用户添加成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        txtError.Text = response?.Message ?? "添加用户失败";
                    }
                }
                else
                {
                    // 编辑用户
                    if (_user != null)
                    {
                        var updatedUser = new User
                        {
                            UserId = _user.UserId,
                            Username = txtUsername.Text.Trim(),
                            Password = _user.Password, // 保持原有密码
                            RealName = txtRealName.Text.Trim(),
                            Email = txtEmail.Text.Trim(),
                            Phone = txtPhone.Text.Trim(),
                            Role = (string)((ComboBoxItem)cmbRole.SelectedItem).Tag,
                            Status = (string)((ComboBoxItem)cmbStatus.SelectedItem).Tag,
                            CreatedAt = _user.CreatedAt,
                            UpdatedAt = DateTime.Now,
                            LastLogin = _user.LastLogin
                        };

                        var response = await _apiService.UpdateUserAsync(updatedUser.UserId, updatedUser);
                        if (response != null && response.Success)
                        {
                            MessageBox.Show("用户更新成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                            DialogResult = true;
                            this.Close();
                        }
                        else
                        {
                            txtError.Text = response?.Message ?? "更新用户失败";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                txtError.Text = "保存用户信息失败，请重试";
                Console.WriteLine($"保存用户异常: {ex.Message}");
            }
            finally
            {
                btnSave.IsEnabled = true;
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

            if (string.IsNullOrWhiteSpace(txtRealName.Text))
            {
                txtError.Text = "请输入真实姓名";
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

            if (_isNewUser)
            {
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
            }

            return true;
        }
    }
}