using System;
using System.Windows;
using FurnitureManagement.Client.Models;
using FurnitureManagement.Client.Servcie;

namespace FurnitureManagement.Client.Views
{
    /// <summary>
    /// WarehouseEditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WarehouseEditWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly Warehouse? _warehouse;
        private readonly bool _isNewWarehouse;

        public WarehouseEditWindow(Warehouse? warehouse = null)
        {
            InitializeComponent();
            _apiService = new ApiService();
            _warehouse = warehouse;
            _isNewWarehouse = warehouse == null;
            InitializeWindow();
        }

        // 初始化窗口
        private void InitializeWindow()
        {
            if (_isNewWarehouse)
            {
                txtTitle.Text = "添加仓库";
            }
            else
            {
                txtTitle.Text = "编辑仓库";
                LoadWarehouseInfo();
            }
        }

        // 加载仓库信息
        private void LoadWarehouseInfo()
        {
            if (_warehouse != null)
            {
                txtWarehouseName.Text = _warehouse.WarehouseName;
                txtLocation.Text = _warehouse.Location ?? string.Empty;
                txtCapacity.Text = _warehouse.Capacity?.ToString() ?? string.Empty;
                txtManager.Text = _warehouse.Manager ?? string.Empty;
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
                if (_isNewWarehouse)
                {
                    // 新建仓库
                    var newWarehouse = new Warehouse
                    {
                        WarehouseName = txtWarehouseName.Text.Trim(),
                        Location = string.IsNullOrWhiteSpace(txtLocation.Text) ? null : txtLocation.Text.Trim(),
                        Capacity = int.TryParse(txtCapacity.Text.Trim(), out var capacity) ? capacity : null,
                        Manager = string.IsNullOrWhiteSpace(txtManager.Text) ? null : txtManager.Text.Trim(),
                        CreatedAt = DateTime.Now
                    };

                    var result = await _apiService.CreateWarehouseAsync(newWarehouse);
                    if (result != null)
                    {
                        MessageBox.Show("仓库添加成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        ShowError("添加仓库失败，请重试");
                    }
                }
                else
                {
                    // 编辑仓库
                    if (_warehouse != null)
                    {
                        var updatedWarehouse = new Warehouse
                        {
                            WarehouseId = _warehouse.WarehouseId,
                            WarehouseName = txtWarehouseName.Text.Trim(),
                            Location = string.IsNullOrWhiteSpace(txtLocation.Text) ? null : txtLocation.Text.Trim(),
                            Capacity = int.TryParse(txtCapacity.Text.Trim(), out var capacity) ? capacity : null,
                            Manager = string.IsNullOrWhiteSpace(txtManager.Text) ? null : txtManager.Text.Trim(),
                            CreatedAt = _warehouse.CreatedAt
                        };

                        var response = await _apiService.UpdateWarehouseAsync(_warehouse.WarehouseId, updatedWarehouse);
                        if (response != null && response.Success)
                        {
                            MessageBox.Show("仓库更新成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.DialogResult = true;
                            this.Close();
                        }
                        else
                        {
                            ShowError(response?.Message ?? "更新仓库失败");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("保存仓库信息失败，请重试");
                Console.WriteLine($"保存仓库异常: {ex.Message}");
            }
            finally
            {
                btnSave.IsEnabled = true;
            }
        }

        // 验证输入
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtWarehouseName.Text))
            {
                ShowError("请输入仓库名称");
                return false;
            }

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
