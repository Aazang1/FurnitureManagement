using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FurnitureManagement.Client.Servcie;
using FurnitureManagement.Client.Models;

namespace FurnitureManagement.Client.Views
{
    /// <summary>
    /// WarehouseManagementPage.xaml 的交互逻辑
    /// </summary>
    public partial class WarehouseManagementPage : Page
    {
        private readonly ApiService _apiService;
        private List<Warehouse>? _allWarehouses;

        public WarehouseManagementPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            // 页面加载时加载仓库列表
            LoadWarehousesAsync();
        }

        // 加载仓库列表
        private async void LoadWarehousesAsync()
        {
            try
            {
                _allWarehouses = await _apiService.GetWarehousesAsync();
                dgWarehouses.ItemsSource = _allWarehouses;
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载仓库列表失败，请重试", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"加载仓库失败: {ex.Message}");
            }
        }

        private void btnAddWarehouse_Click(object sender, RoutedEventArgs e)
        {
            // 添加仓库逻辑 - 传入null表示新建
            var editWindow = new WarehouseEditWindow(null);
            if (editWindow.ShowDialog() == true)
            {
                // 刷新仓库列表
                LoadWarehousesAsync();
            }
        }

        private void btnEditWarehouse_Click(object sender, RoutedEventArgs e)
        {
            // 编辑仓库逻辑 - 传入选中的仓库
            var button = sender as Button;
            var warehouse = button?.Tag as Warehouse;
            if (warehouse != null)
            {
                var editWindow = new WarehouseEditWindow(warehouse);
                if (editWindow.ShowDialog() == true)
                {
                    // 刷新仓库列表
                    LoadWarehousesAsync();
                }
            }
        }

        private async void btnDeleteWarehouse_Click(object sender, RoutedEventArgs e)
        {
            // 删除仓库逻辑
            var button = sender as Button;
            var warehouse = button?.Tag as Warehouse;
            if (warehouse != null)
            {
                var result = MessageBox.Show($"确定要删除仓库 {warehouse.WarehouseName} 吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var response = await _apiService.DeleteWarehouseAsync(warehouse.WarehouseId);
                        if (response != null && response.Success)
                        {
                            MessageBox.Show("仓库删除成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadWarehousesAsync();
                        }
                        else
                        {
                            MessageBox.Show(response?.Message ?? "删除失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("删除仓库失败，请重试", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        Console.WriteLine($"删除仓库异常: {ex.Message}");
                    }
                }
            }
        }
    }
}
