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
    /// SupplierManagementPage.xaml 的交互逻辑
    /// </summary>
    public partial class SupplierManagementPage : Page
    {
        private readonly ApiService _apiService;
        private List<Supplier>? _allSuppliers;

        public SupplierManagementPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            // 页面加载时加载供应商列表
            LoadSuppliersAsync();
        }

        // 加载供应商列表
        private async void LoadSuppliersAsync()
        {
            try
            {
                _allSuppliers = await _apiService.GetSuppliersAsync();
                dgSuppliers.ItemsSource = _allSuppliers;
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载供应商列表失败，请重试", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"加载供应商失败: {ex.Message}");
            }
        }

        private void btnAddSupplier_Click(object sender, RoutedEventArgs e)
        {
            // 添加供应商逻辑 - 传入null表示新建
            var editWindow = new SupplierEditWindow(null);
            if (editWindow.ShowDialog() == true)
            {
                // 刷新供应商列表
                LoadSuppliersAsync();
            }
        }

        private void btnEditSupplier_Click(object sender, RoutedEventArgs e)
        {
            // 编辑供应商逻辑 - 传入选中的供应商
            var button = sender as Button;
            var supplier = button?.Tag as Supplier;
            if (supplier != null)
            {
                var editWindow = new SupplierEditWindow(supplier);
                if (editWindow.ShowDialog() == true)
                {
                    // 刷新供应商列表
                    LoadSuppliersAsync();
                }
            }
        }

        private async void btnDeleteSupplier_Click(object sender, RoutedEventArgs e)
        {
            // 删除供应商逻辑
            var button = sender as Button;
            var supplier = button?.Tag as Supplier;
            if (supplier != null)
            {
                var result = MessageBox.Show($"确定要删除供应商 {supplier.SupplierName} 吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var response = await _apiService.DeleteSupplierAsync(supplier.SupplierId);
                        if (response != null && response.Success)
                        {
                            MessageBox.Show("供应商删除成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadSuppliersAsync();
                        }
                        else
                        {
                            MessageBox.Show(response?.Message ?? "删除失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("删除供应商失败，请重试", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        Console.WriteLine($"删除供应商异常: {ex.Message}");
                    }
                }
            }
        }
    }
}
