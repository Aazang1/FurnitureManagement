using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;using System.Windows;using System.Windows.Controls;using System.Windows.Data;using System.Windows.Documents;using System.Windows.Input;using System.Windows.Media;using System.Windows.Media.Imaging;using System.Windows.Navigation;using System.Windows.Shapes;
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
            // 添加供应商逻辑
            var editWindow = new SupplierEditWindow();
            if (editWindow.ShowDialog() == true)
            {
                // 刷新供应商列表
                LoadSuppliersAsync();
            }
        }

        private void btnEditSupplier_Click(object sender, RoutedEventArgs e)
        {
            // 编辑供应商逻辑
            var button = sender as Button;
            var supplier = button?.Tag as Supplier;
            if (supplier != null)
            {
                var editWindow = new SupplierEditWindow();
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
                        await _apiService.DeleteSupplierAsync(supplier.SupplierId);
                        MessageBox.Show("供应商删除成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadSuppliersAsync();
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