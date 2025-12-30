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
                cmbRole.SelectedIndex = 2; // 默认选择普通用户 (User)
                cmbStatus.SelectedIndex = 0; // 默认状态为启用 (active)
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
                        RealName = txtRealName.Text.Trim(),
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
            // 用户名验证
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                txtError.Text = "请输入用户名";
                txtUsername.Focus();
                return false;
            }

            if (txtUsername.Text.Trim().Length < 3)
            {
                txtError.Text = "用户名长度不能少于3个字符";
                txtUsername.Focus();
                return false;
            }

            // 真实姓名验证
            if (string.IsNullOrWhiteSpace(txtRealName.Text))
            {
                txtError.Text = "请输入真实姓名";
                txtRealName.Focus();
                return false;
            }

            // 邮箱验证
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                txtError.Text = "请输入邮箱";
                txtEmail.Focus();
                return false;
            }

            if (!Regex.IsMatch(txtEmail.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                txtError.Text = "请输入有效的邮箱地址";
                txtEmail.Focus();
                return false;
            }

            // 手机号验证（可选，但如果填写了需要验证格式）
            if (!string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                if (!Regex.IsMatch(txtPhone.Text.Trim(), @"^1[3-9]\d{9}$"))
                {
                    txtError.Text = "请输入有效的手机号码";
                    txtPhone.Focus();
                    return false;
                }
            }

            // 角色验证
            if (cmbRole.SelectedItem == null)
            {
                txtError.Text = "请选择用户角色";
                return false;
            }

            // 状态验证
            if (cmbStatus.SelectedItem == null)
            {
                txtError.Text = "请选择用户状态";
                return false;
            }

            // 新建用户时验证密码
            if (_isNewUser)
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    txtError.Text = "请输入密码";
                    txtPassword.Focus();
                    return false;
                }

                if (txtPassword.Password.Length < 6)
                {
                    txtError.Text = "密码长度不能少于6位";
                    txtPassword.Focus();
                    return false;
                }

                // 密码复杂度验证（可选）
                if (!Regex.IsMatch(txtPassword.Password, @"^(?=.*[a-zA-Z])(?=.*\d).+$"))
                {
                    txtError.Text = "密码必须包含字母和数字";
                    txtPassword.Focus();
                    return false;
                }
            }

            return true;
        }
    }
}
