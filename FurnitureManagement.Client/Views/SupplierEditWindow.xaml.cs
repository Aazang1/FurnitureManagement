using System;
using System.Windows;
using FurnitureManagement.Client.Models;
using FurnitureManagement.Client.Servcie;

namespace FurnitureManagement.Client.Views
{
    /// <summary>
    /// SupplierEditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SupplierEditWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly Supplier? _supplier;
        private readonly bool _isNewSupplier;

        public SupplierEditWindow(Supplier? supplier = null)
        {
            InitializeComponent();
            _apiService = new ApiService();
            _supplier = supplier;
            _isNewSupplier = supplier == null;
            InitializeWindow();
        }

        // 初始化窗口
        private void InitializeWindow()
        {
            if (_isNewSupplier)
            {
                txtTitle.Text = "添加供应商";
            }
            else
            {
                txtTitle.Text = "编辑供应商";
                LoadSupplierInfo();
            }
        }

        // 加载供应商信息
        private void LoadSupplierInfo()
        {
            if (_supplier != null)
            {
                txtSupplierName.Text = _supplier.SupplierName;
                txtContactPerson.Text = _supplier.ContactPerson ?? string.Empty;
                txtPhone.Text = _supplier.Phone ?? string.Empty;
                txtAddress.Text = _supplier.Address ?? string.Empty;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // 验证输入
            if (!ValidateInput())
                return;

            btnSave.IsEnabled = false;
            txtError.Visibility = Visibility.Collapsed;

            try
            {
                if (_isNewSupplier)
                {
                    // 新建供应商
                    var newSupplier = new Supplier
                    {
                        SupplierName = txtSupplierName.Text.Trim(),
                        ContactPerson = string.IsNullOrWhiteSpace(txtContactPerson.Text) ? null : txtContactPerson.Text.Trim(),
                        Phone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim(),
                        Address = string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text.Trim(),
                        CreatedAt = DateTime.Now
                    };

                    var result = await _apiService.CreateSupplierAsync(newSupplier);
                    if (result != null)
                    {
                        MessageBox.Show("供应商添加成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        ShowError("添加供应商失败，请重试");
                    }
                }
                else
                {
                    // 编辑供应商
                    if (_supplier != null)
                    {
                        var updatedSupplier = new Supplier
                        {
                            SupplierId = _supplier.SupplierId,
                            SupplierName = txtSupplierName.Text.Trim(),
                            ContactPerson = string.IsNullOrWhiteSpace(txtContactPerson.Text) ? null : txtContactPerson.Text.Trim(),
                            Phone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim(),
                            Address = string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text.Trim(),
                            CreatedAt = _supplier.CreatedAt
                        };

                        var response = await _apiService.UpdateSupplierAsync(_supplier.SupplierId, updatedSupplier);
                        if (response != null && response.Success)
                        {
                            MessageBox.Show("供应商更新成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.DialogResult = true;
                            this.Close();
                        }
                        else
                        {
                            ShowError(response?.Message ?? "更新供应商失败");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("保存供应商信息失败，请重试");
                Console.WriteLine($"保存供应商异常: {ex.Message}");
            }
            finally
            {
                btnSave.IsEnabled = true;
            }
        }

        // 验证输入
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtSupplierName.Text))
            {
                ShowError("请输入供应商名称");
                return false;
            }

            // 其他字段为可选项，不需要强制验证
            return true;
        }

        // 显示错误信息
        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Visibility = Visibility.Visible;
        }
    }
}
