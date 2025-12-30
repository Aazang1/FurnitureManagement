using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FurnitureManagement.Client.Models;
using FurnitureManagement.Client.Servcie;

namespace FurnitureManagement.Client.Views
{
    /// <summary>
    /// InventoryManagementPage.xaml 的交互逻辑
    /// </summary>
    public partial class InventoryManagementPage : Page
    {
        private readonly ApiService _apiService;
        private List<Inventory>? _allInventory;
        private List<Furniture>? _allProducts;
        private List<Warehouse>? _allWarehouses;

        // 分页相关字段
        private List<Inventory> _pagedInventory;
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalPages = 1;

        public InventoryManagementPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            InitializePaginationControls();
            LoadDataAsync();
        }

        // 初始化分页控件
        private void InitializePaginationControls()
        {
            cmbPageSize.SelectedIndex = 1; // 默认选择10条
            cmbPageSize.SelectionChanged += cmbPageSize_SelectionChanged;
        }

        // 分页相关属性
        private List<Inventory> PagedInventory
        {
            get { return _pagedInventory; }
            set
            {
                _pagedInventory = value;
                // 修复：移除可能导致递归的UI更新，只在需要时调用
            }
        }

        private int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                // 修复：移除可能导致递归的UI更新
            }
        }

        private int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
                CurrentPage = 1; // 重置到第一页
            }
        }

        private int TotalPages
        {
            get { return _totalPages; }
            set
            {
                _totalPages = value;
                // 修复：移除可能导致递归的UI更新
            }
        }

        // 更新分页UI - 修复版本，避免无限递归
        private void UpdatePaginationUI()
        {
            if (_allInventory != null)
            {
                // 计算总页数
                int newTotalPages = (int)Math.Ceiling(_allInventory.Count / (double)PageSize);
                _totalPages = newTotalPages;
                
                // 确保当前页在有效范围内
                if (CurrentPage > TotalPages && TotalPages > 0)
                    _currentPage = TotalPages;
                if (CurrentPage < 1)
                    _currentPage = 1;

                // 计算当前页的数据范围
                int startIndex = (CurrentPage - 1) * PageSize;
                int endIndex = Math.Min(startIndex + PageSize, _allInventory.Count);

                // 获取当前页的数据
                _pagedInventory = _allInventory.Skip(startIndex).Take(PageSize).ToList();
                dgInventory.ItemsSource = _pagedInventory;

                // 更新分页信息显示
                txtPageInfo.Text = $"第{CurrentPage}页/共{TotalPages}页";
                txtTotalInfo.Text = $"共{_allInventory.Count}条记录";

                // 更新按钮状态
                btnFirstPage.IsEnabled = CurrentPage > 1;
                btnPrevPage.IsEnabled = CurrentPage > 1;
                btnNextPage.IsEnabled = CurrentPage < TotalPages;
                btnLastPage.IsEnabled = CurrentPage < TotalPages;

                // 设置按钮样式
                SetButtonState(btnFirstPage, CurrentPage > 1);
                SetButtonState(btnPrevPage, CurrentPage > 1);
                SetButtonState(btnNextPage, CurrentPage < TotalPages);
                SetButtonState(btnLastPage, CurrentPage < TotalPages);
            }
        }

        // 设置按钮状态样式
        private void SetButtonState(Button button, bool enabled)
        {
            if (enabled)
            {
                // 设置为启用状态的样式
                button.Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(149, 165, 166)); // #95A5A6
            }
            else
            {
                // 设置为禁用状态的样式
                button.Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(189, 195, 199)); // #BDC3C7
            }
        }

        // 加载数据
        private async void LoadDataAsync()
        {
            try
            {
                // 并行加载库存、商品和仓库数据
                var inventoryTask = _apiService.GetInventoryAsync();
                var productsTask = _apiService.GetFurnitureAsync();
                var warehousesTask = _apiService.GetWarehousesAsync();
                
                await Task.WhenAll(inventoryTask, productsTask, warehousesTask);
                
                _allInventory = await inventoryTask;
                _allProducts = await productsTask;
                _allWarehouses = await warehousesTask;
                
                if (_allInventory != null)
                {
                    // 关联库存与商品和仓库
                    foreach (var inventory in _allInventory)
                    {
                        if (_allProducts != null)
                        {
                            inventory.Furniture = _allProducts.FirstOrDefault(p => p.FurnitureId == inventory.FurnitureId);
                        }
                        if (_allWarehouses != null)
                        {
                            inventory.Warehouse = _allWarehouses.FirstOrDefault(w => w.WarehouseId == inventory.WarehouseId);
                        }
                    }
                    // 修复：数据加载完成后主动更新分页UI
                    _currentPage = 1; // 重置到第一页
                    UpdatePaginationUI(); // 主动调用更新UI
                }
                else
                {
                    // 如果没有数据，也要清空显示
                    dgInventory.ItemsSource = null;
                    txtPageInfo.Text = "第1页/共1页";
                    txtTotalInfo.Text = "共0条记录";
                    
                    // 禁用分页按钮
                    btnFirstPage.IsEnabled = false;
                    btnPrevPage.IsEnabled = false;
                    btnNextPage.IsEnabled = false;
                    btnLastPage.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载数据失败，请重试", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"加载库存数据失败: {ex.Message}");
            }
        }

        // 每页显示条数变更事件
        private void cmbPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbPageSize.SelectedItem is ComboBoxItem selectedItem)
            {
                string content = selectedItem.Content.ToString();
                if (int.TryParse(content.Replace("条", ""), out int newPageSize))
                {
                    PageSize = newPageSize;
                    UpdatePaginationUI();
                }
            }
        }

        // 分页按钮事件处理
        private void btnFirstPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = 1;
            UpdatePaginationUI();
        }

        private void btnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                UpdatePaginationUI();
            }
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                UpdatePaginationUI();
            }
        }

        private void btnLastPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = TotalPages;
            UpdatePaginationUI();
        }

        // 添加库存按钮点击事件
        private void btnAddInventory_Click(object sender, RoutedEventArgs e)
        {
            // 打开库存编辑窗口（新建库存）
            var editWindow = new InventoryEditWindow(null, _allProducts, _allWarehouses);
            editWindow.Closed += (s, args) =>
            {
                // 窗口关闭后重新加载数据
                LoadDataAsync();
            };
            editWindow.ShowDialog();
        }

        // 编辑库存按钮点击事件
        private void btnEditInventory_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var inventory = button?.Tag as Inventory;
            if (inventory != null)
            {
                // 打开库存编辑窗口（编辑现有库存）
                var editWindow = new InventoryEditWindow(inventory, _allProducts, _allWarehouses);
                editWindow.Closed += (s, args) =>
                {
                    // 窗口关闭后重新加载数据
                    LoadDataAsync();
                };
                editWindow.ShowDialog();
            }
        }

        // 删除库存按钮点击事件
        private async void btnDeleteInventory_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var inventory = button?.Tag as Inventory;
            if (inventory != null)
            {
                // 显示确认对话框
                var result = MessageBox.Show($"确定要删除库存记录吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var response = await _apiService.DeleteInventoryAsync(inventory.InventoryId);
                        if (response != null && response.Success)
                        {
                            MessageBox.Show("库存删除成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadDataAsync();
                        }
                        else
                        {
                            MessageBox.Show(response?.Message ?? "删除失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("删除库存失败，请重试", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        Console.WriteLine($"删除库存异常: {ex.Message}");
                    }
                }
            }
        }

        // 调整数量按钮点击事件
        private async void btnAdjustQuantity_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var inventory = button?.Tag as Inventory;
            if (inventory != null)
            {
                // 显示调整数量对话框
                var quantityInput = Microsoft.VisualBasic.Interaction.InputBox("请输入新的数量:", "调整库存数量", inventory.Quantity.ToString());
                if (!string.IsNullOrWhiteSpace(quantityInput) && int.TryParse(quantityInput, out int newQuantity))
                {
                    if (newQuantity < 0)
                    {
                        MessageBox.Show("数量不能为负数", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    
                    try
                    {
                        var response = await _apiService.UpdateInventoryQuantityAsync(inventory.InventoryId, newQuantity);
                        if (response != null && response.Success)
                        {
                            MessageBox.Show("库存数量更新成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadDataAsync();
                        }
                        else
                        {
                            MessageBox.Show(response?.Message ?? "更新失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("更新库存数量失败，请重试", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        Console.WriteLine($"更新库存数量异常: {ex.Message}");
                    }
                }
            }
        }
    }
}