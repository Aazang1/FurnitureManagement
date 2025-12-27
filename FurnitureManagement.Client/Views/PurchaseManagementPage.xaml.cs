using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FurnitureManagement.Client.Models;
using FurnitureManagement.Client.Servcie;

namespace FurnitureManagement.Client.Views
{
    public partial class PurchaseManagementPage : Page
    {
        private readonly ApiService _apiService;
        private List<PurchaseOrder>? _purchaseOrders;
        private List<Supplier>? _suppliers;
        private List<Furniture>? _furnitureList;

        public PurchaseManagementPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            LoadDataAsync();
        }

        // 加载数据
        private async void LoadDataAsync()
        {
            await LoadPurchaseOrdersAsync();
            await LoadSuppliersAsync();
            await LoadFurnitureAsync();
        }

        // 加载采购订单列表
        private async Task LoadPurchaseOrdersAsync()
        {
            _purchaseOrders = await _apiService.GetPurchaseOrdersAsync();
            dgPurchaseOrders.ItemsSource = _purchaseOrders;
        }

        // 加载供应商列表
        private async Task LoadSuppliersAsync()
        {
            _suppliers = await _apiService.GetSuppliersAsync();
            if (_suppliers != null)
            {
                // 添加"所有供应商"选项
                var allSuppliers = new List<Supplier>
                {
                    new Supplier { SupplierId = 0, SupplierName = "所有供应商" }
                };
                allSuppliers.AddRange(_suppliers);

                cmbSupplier.ItemsSource = allSuppliers;
                cmbSupplier.DisplayMemberPath = "SupplierName";
                cmbSupplier.SelectedValuePath = "SupplierId";
                cmbSupplier.SelectedIndex = 0; // 默认选择"所有供应商"
            }
        }

        // 加载商品列表
        private async Task LoadFurnitureAsync()
        {
            _furnitureList = await _apiService.GetFurnitureAsync();
            if (_furnitureList != null)
            {
                // 添加"所有商品"选项
                var allFurniture = new List<Furniture>
                {
                    new Furniture { FurnitureId = 0, FurnitureName = "所有商品" }
                };
                allFurniture.AddRange(_furnitureList);

                cmbFurniture.ItemsSource = allFurniture;
                cmbFurniture.DisplayMemberPath = "FurnitureName";
                cmbFurniture.SelectedValuePath = "FurnitureId";
                cmbFurniture.SelectedIndex = 0; // 默认选择"所有商品"
            }
        }

        // 创建采购订单
        private void btnCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            var createWindow = new CreatePurchaseOrderWindow();
            createWindow.Owner = Window.GetWindow(this);
            if (createWindow.ShowDialog() == true)
            {
                // 刷新采购订单列表
                LoadPurchaseOrdersAsync();
            }
        }

        // 查看采购明细
        private void btnViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is PurchaseOrder order)
            {
                var detailWindow = new PurchaseDetailWindow(order);
                detailWindow.Owner = Window.GetWindow(this);
                detailWindow.ShowDialog();
            }
        }

        // 完成采购订单
        private async void btnCompleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is PurchaseOrder order)
            {
                // 检查订单状态是否为"待处理"
                if (order.Status == "completed")
                {
                    MessageBox.Show("此订单已完成，无需重复操作", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else if (order.Status == "cancelled")
                {
                    MessageBox.Show("此订单已取消，无法完成", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var result = MessageBox.Show($"确定要完成采购订单 {order.PurchaseOrderId} 吗？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var response = await _apiService.CompletePurchaseOrderAsync(order.PurchaseOrderId);
                    if (response != null && response.Success)
                    {
                        MessageBox.Show("采购订单已完成，库存已更新", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadPurchaseOrdersAsync();
                    }
                    else
                    {
                        MessageBox.Show(response?.Message ?? "完成采购订单失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // 取消采购订单
        private async void btnCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is PurchaseOrder order)
            {
                // 检查订单状态
                if (order.Status == "completed")
                {
                    MessageBox.Show("此订单已完成，无法取消", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else if (order.Status == "cancelled")
                {
                    MessageBox.Show("此订单已取消，无需重复操作", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var result = MessageBox.Show($"确定要取消采购订单 {order.PurchaseOrderId} 吗？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var response = await _apiService.CancelPurchaseOrderAsync(order.PurchaseOrderId);
                    if (response != null && response.Success)
                    {
                        MessageBox.Show("采购订单已取消", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadPurchaseOrdersAsync();
                    }
                    else
                    {
                        MessageBox.Show(response?.Message ?? "取消采购订单失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // 按日期筛选采购订单
        private void btnFilterByDate_Click(object sender, RoutedEventArgs e)
        {
            FilterPurchaseOrders();
        }

        // 重置筛选条件
        private void btnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            dpFromDate.SelectedDate = null;
            dpToDate.SelectedDate = null;
            cmbSupplier.SelectedIndex = 0;
            cmbFurniture.SelectedIndex = 0;
            dgPurchaseOrders.ItemsSource = _purchaseOrders;
        }

        // 按供应商查询采购订单
        private async void btnSearchBySupplier_Click(object sender, RoutedEventArgs e)
        {
            await FilterPurchaseOrdersBySupplierAsync();
        }

        // 按商品查询采购订单
        private async void btnSearchByFurniture_Click(object sender, RoutedEventArgs e)
        {
            await FilterPurchaseOrdersByFurnitureAsync();
        }

        // 综合筛选方法
        private async void FilterPurchaseOrders()
        {
            if (_purchaseOrders == null) return;

            IEnumerable<PurchaseOrder> filteredOrders = _purchaseOrders;

            // 按日期筛选
            if (dpFromDate.SelectedDate.HasValue)
            {
                filteredOrders = filteredOrders.Where(po => po.PurchaseDate.Date >= dpFromDate.SelectedDate.Value.Date);
            }

            if (dpToDate.SelectedDate.HasValue)
            {
                filteredOrders = filteredOrders.Where(po => po.PurchaseDate.Date <= dpToDate.SelectedDate.Value.Date);
            }

            // 按供应商筛选
            if (cmbSupplier.SelectedValue is not null && cmbSupplier.SelectedValue.ToString() is string strSupplierValue &&
                int.TryParse(strSupplierValue, out int supplierId) && supplierId > 0)
            {
                if (supplierId > 0) // 如果不是"所有供应商"
                {
                    filteredOrders = filteredOrders.Where(po => po.Supplier != null && po.Supplier.SupplierId == supplierId);
                }
            }

            // 按商品筛选
            if (cmbFurniture.SelectedValue is not null && cmbFurniture.SelectedValue.ToString() is string strFurnitureValue &&
                int.TryParse(strFurnitureValue, out int furnitureId) && furnitureId > 0)
            {
                if (furnitureId > 0) // 如果不是"所有商品"
                {
                    // 这里需要获取包含特定商品的采购订单
                    var ordersWithFurniture = await _apiService.GetPurchaseOrdersByFurnitureAsync(furnitureId);
                    if (ordersWithFurniture != null)
                    {
                        var orderIds = ordersWithFurniture.Select(o => o.PurchaseOrderId).ToList();
                        filteredOrders = filteredOrders.Where(po => orderIds.Contains(po.PurchaseOrderId));
                    }
                }
            }

            dgPurchaseOrders.ItemsSource = filteredOrders.ToList();
        }

        // 按供应商筛选
        private async Task FilterPurchaseOrdersBySupplierAsync()
        {
            if (_purchaseOrders == null) return;

            if (cmbSupplier.SelectedValue is not null && cmbSupplier.SelectedValue.ToString() is string strValue &&
                int.TryParse(strValue, out int supplierId))
            {
                IEnumerable<PurchaseOrder> filteredOrders = _purchaseOrders;

                if (supplierId == 0) // 所有供应商
                {
                    dgPurchaseOrders.ItemsSource = _purchaseOrders;
                }
                else
                {
                    // 首先按供应商筛选
                    filteredOrders = filteredOrders.Where(po => po.Supplier != null && po.Supplier.SupplierId == supplierId);

                    // 然后按日期筛选
                    if (dpFromDate.SelectedDate.HasValue)
                    {
                        filteredOrders = filteredOrders.Where(po => po.PurchaseDate.Date >= dpFromDate.SelectedDate.Value.Date);
                    }

                    if (dpToDate.SelectedDate.HasValue)
                    {
                        filteredOrders = filteredOrders.Where(po => po.PurchaseDate.Date <= dpToDate.SelectedDate.Value.Date);
                    }

                    dgPurchaseOrders.ItemsSource = filteredOrders.ToList();
                }
            }
        }

        // 按商品筛选
        private async Task FilterPurchaseOrdersByFurnitureAsync()
        {
            if (_purchaseOrders == null) return;

            if (cmbFurniture.SelectedValue is not null && cmbFurniture.SelectedValue.ToString() is string strValue &&
                int.TryParse(strValue, out int furnitureId))
            {
                if (furnitureId == 0) // 所有商品
                {
                    FilterPurchaseOrders();
                }
                else
                {
                    // 获取包含特定商品的采购订单
                    var orders = await _apiService.GetPurchaseOrdersByFurnitureAsync(furnitureId);
                    if (orders != null)
                    {
                        var orderIds = orders.Select(o => o.PurchaseOrderId).ToList();
                        var filteredOrders = _purchaseOrders.Where(po => orderIds.Contains(po.PurchaseOrderId)).ToList();

                        // 再按日期筛选
                        if (dpFromDate.SelectedDate.HasValue)
                        {
                            filteredOrders = filteredOrders.Where(po => po.PurchaseDate.Date >= dpFromDate.SelectedDate.Value.Date).ToList();
                        }

                        if (dpToDate.SelectedDate.HasValue)
                        {
                            filteredOrders = filteredOrders.Where(po => po.PurchaseDate.Date <= dpToDate.SelectedDate.Value.Date).ToList();
                        }

                        dgPurchaseOrders.ItemsSource = filteredOrders;
                    }
                }
            }
        }

        private void dgPurchaseOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 可以添加行选择逻辑
        }
    }
}