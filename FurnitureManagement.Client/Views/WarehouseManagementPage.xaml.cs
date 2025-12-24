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
                Console.WriteLine("开始加载仓库列表...");
                _allWarehouses = await _apiService.GetWarehousesAsync();
                
                if (_allWarehouses == null)
                {
                    Console.WriteLine("API返回null，可能是网络连接问题或服务端错误");
                    MessageBox.Show("无法连接到服务器，请检查服务端是否正在运行", "连接错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                Console.WriteLine($"成功加载 {_allWarehouses.Count} 个仓库");
                dgWarehouses.ItemsSource = _allWarehouses;
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载仓库失败: {ex.Message}");
                Console.WriteLine($"详细错误: {ex}");
                MessageBox.Show($"加载仓库列表失败：{ex.Message}\n\n请检查：\n1. 服务端是否正在运行\n2. 数据库连接是否正常\n3. 网络连接是否正常", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 更新统计信息
        private void UpdateStatistics()
        {
            if (_allWarehouses == null)
            {
                txtTotalWarehouses.Text = "0";
                txtTotalCapacity.Text = "0";
                txtAverageCapacity.Text = "0";
                txtSearchResults.Text = "0";
                return;
            }

            // 总仓库数
            txtTotalWarehouses.Text = _allWarehouses.Count.ToString();

            // 总容量
            var totalCapacity = _allWarehouses.Where(w => w.Capacity.HasValue).Sum(w => w.Capacity.Value);
            txtTotalCapacity.Text = totalCapacity.ToString();

            // 平均容量
            var warehousesWithCapacity = _allWarehouses.Where(w => w.Capacity.HasValue).ToList();
            var averageCapacity = warehousesWithCapacity.Count > 0 ? warehousesWithCapacity.Average(w => w.Capacity.Value) : 0;
            txtAverageCapacity.Text = Math.Round(averageCapacity, 0).ToString();

            // 搜索结果数量（初始为全部）
            txtSearchResults.Text = _allWarehouses.Count.ToString();
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
            if (_allWarehouses == null)
                return;

            var filteredWarehouses = _allWarehouses.AsEnumerable();

            // 应用搜索筛选（按仓库名称、位置、管理员）
            string searchText = txtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filteredWarehouses = filteredWarehouses.Where(w => 
                    w.WarehouseName.ToLower().Contains(searchText) ||
                    (!string.IsNullOrEmpty(w.Location) && w.Location.ToLower().Contains(searchText)) ||
                    (!string.IsNullOrEmpty(w.Manager) && w.Manager.ToLower().Contains(searchText))
                );
            }

            var resultList = filteredWarehouses.ToList();
            dgWarehouses.ItemsSource = resultList;
            
            // 更新搜索结果统计
            txtSearchResults.Text = resultList.Count.ToString();
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
