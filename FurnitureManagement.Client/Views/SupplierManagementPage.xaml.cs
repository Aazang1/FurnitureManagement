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
                Console.WriteLine("开始加载供应商列表...");
                _allSuppliers = await _apiService.GetSuppliersAsync();
                
                if (_allSuppliers == null)
                {
                    Console.WriteLine("API返回null，可能是网络连接问题或服务端错误");
                    MessageBox.Show("无法连接到服务器，请检查服务端是否正在运行", "连接错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                Console.WriteLine($"成功加载 {_allSuppliers.Count} 个供应商");
                dgSuppliers.ItemsSource = _allSuppliers;
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载供应商失败: {ex.Message}");
                Console.WriteLine($"详细错误: {ex}");
                MessageBox.Show($"加载供应商列表失败：{ex.Message}\n\n请检查：\n1. 服务端是否正在运行\n2. 数据库连接是否正常\n3. 网络连接是否正常", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 更新统计信息
        private void UpdateStatistics()
        {
            if (_allSuppliers == null)
            {
                txtTotalSuppliers.Text = "0";
                txtActiveSuppliers.Text = "0";
                txtMonthlyNew.Text = "0";
                txtSearchResults.Text = "0";
                return;
            }

            // 总供应商数
            txtTotalSuppliers.Text = _allSuppliers.Count.ToString();

            // 活跃供应商（假设所有供应商都是活跃的，实际可以根据业务逻辑调整）
            txtActiveSuppliers.Text = _allSuppliers.Count.ToString();

            // 本月新增供应商
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            var monthlyNew = _allSuppliers.Count(s => s.CreatedAt.Month == currentMonth && s.CreatedAt.Year == currentYear);
            txtMonthlyNew.Text = monthlyNew.ToString();

            // 搜索结果数量（初始为全部）
            txtSearchResults.Text = _allSuppliers.Count.ToString();
        }

        // 搜索按钮点击事件
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        // 实时搜索事件
        private void txtSearch_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            ApplyFilters();
        }

        // 清除筛选按钮点击事件
        private void btnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = string.Empty;
            ApplyFilters();
        }

        // 应用搜索和筛选
        private void ApplyFilters()
        {
            if (_allSuppliers == null)
                return;

            var filteredSuppliers = _allSuppliers.AsEnumerable();

            // 应用搜索筛选（按供应商名称、联系人、电话）
            string searchText = txtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filteredSuppliers = filteredSuppliers.Where(s => 
                    s.SupplierName.ToLower().Contains(searchText) ||
                    (!string.IsNullOrEmpty(s.ContactPerson) && s.ContactPerson.ToLower().Contains(searchText)) ||
                    (!string.IsNullOrEmpty(s.Phone) && s.Phone.ToLower().Contains(searchText))
                );
            }

            var resultList = filteredSuppliers.ToList();
            dgSuppliers.ItemsSource = resultList;
            
            // 更新搜索结果统计
            txtSearchResults.Text = resultList.Count.ToString();
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
