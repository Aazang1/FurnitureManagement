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

        public InventoryManagementPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            LoadDataAsync();
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
                    dgInventory.ItemsSource = _allInventory;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载数据失败，请重试", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"加载库存数据失败: {ex.Message}");
            }
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