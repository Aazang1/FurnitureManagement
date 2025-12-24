using FurnitureManagement.Client.Models;
using FurnitureManagement.Client.Servcie;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace FurnitureManagement.Client.Views
{
    /// <summary>
    /// SaleOrderEditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SaleOrderEditWindow : Window
    {
        private readonly ApiService _apiService;
        private int _saleOrderId;
        private bool _isViewMode;
        private ObservableCollection<SaleDetail> _saleDetails;
        private List<Furniture>? _furnitures;
        private List<Warehouse>? _warehouses;
        private int _currentUserId;

        public SaleOrderEditWindow(int saleOrderId = 0, bool isViewMode = false, int currentUserId = 0)
        {
            InitializeComponent();
            
            _apiService = new ApiService();
            _saleOrderId = saleOrderId;
            _isViewMode = isViewMode;
            _currentUserId = currentUserId;
            _saleDetails = new ObservableCollection<SaleDetail>();
            // 暂时不设置ItemsSource，等数据加载完成后再设置
            // dgSaleDetails.ItemsSource = _saleDetails;
            
            // 初始化日期为当前日期
            dpSaleDate.SelectedDate = DateTime.Now;
            
            // 设置查看模式
            chkIsViewMode.IsChecked = _isViewMode;
            
            if (_isViewMode)
            {
                txtTitle.Text = "查看销售订单";
                btnSave.Visibility = Visibility.Collapsed;
            }
            else if (_saleOrderId > 0)
            {
                txtTitle.Text = "编辑销售订单";
            }
            else
            {
                txtTitle.Text = "添加销售订单";
                cmbStatus.SelectedIndex = 0; // 默认选择 pending
            }
            
            LoadData();
        }

        private async void LoadData()
        {
            // 加载家具和仓库数据
            await LoadFurnituresAndWarehouses();
            
            if (_saleOrderId > 0)
            {
                // 加载销售订单数据
                await LoadSaleOrder();
            }
            
            // 数据加载完成后设置ItemsSource
            dgSaleDetails.ItemsSource = _saleDetails;
            
            // 初始化金额显示
            UpdateAmounts();
        }

        private async Task LoadFurnituresAndWarehouses()
        {
            _furnitures = await _apiService.GetFurnitureAsync() ?? new List<Furniture>();
            _warehouses = await _apiService.GetWarehousesAsync() ?? new List<Warehouse>();
            
            // 添加调试信息
            Console.WriteLine($"加载家具数量: {_furnitures.Count}");
            Console.WriteLine($"加载仓库数量: {_warehouses.Count}");
            
            // 数据加载完成后，更新所有现有行的下拉框数据源
            UpdateAllRowComboBoxes();
        }
        
        private void UpdateAllRowComboBoxes()
        {
            // 确保DataGrid已经生成了所有行的容器
            dgSaleDetails.UpdateLayout();
            
            // 更新所有现有行的下拉框数据源
            for (int i = 0; i < dgSaleDetails.Items.Count; i++)
            {
                var row = dgSaleDetails.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                if (row == null)
                {
                    // 如果行容器尚未生成，则生成它
                    row = dgSaleDetails.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                    if (row == null)
                    {
                        // 如果仍然无法获取行容器，跳过该行
                        continue;
                    }
                }
                
                var item = dgSaleDetails.Items[i] as SaleDetail;
                if (item != null)
                {
                    var cmbFurniture = FindVisualChild<ComboBox>(row, "cmbFurniture");
                    if (cmbFurniture != null)
                    {
                        cmbFurniture.ItemsSource = _furnitures ?? new List<Furniture>();
                        if (item.FurnitureId > 0)
                        {
                            cmbFurniture.SelectedItem = _furnitures?.FirstOrDefault(f => f.FurnitureId == item.FurnitureId);
                        }
                    }
                    
                    var cmbWarehouse = FindVisualChild<ComboBox>(row, "cmbWarehouse");
                    if (cmbWarehouse != null)
                    {
                        cmbWarehouse.ItemsSource = _warehouses ?? new List<Warehouse>();
                        if (item.WarehouseId > 0)
                        {
                            cmbWarehouse.SelectedItem = _warehouses?.FirstOrDefault(w => w.WarehouseId == item.WarehouseId);
                        }
                    }
                }
            }
        }

        private async Task LoadSaleOrder()
        {
            var saleOrder = await _apiService.GetSaleOrderByIdAsync(_saleOrderId);
            if (saleOrder != null)
            {
                // 填充订单基本信息
                txtCustomerName.Text = saleOrder.CustomerName;
                txtCustomerPhone.Text = saleOrder.CustomerPhone;
                dpSaleDate.SelectedDate = saleOrder.SaleDate;
                txtTotalAmount.Text = saleOrder.TotalAmount.ToString("C2");
                txtDiscount.Text = saleOrder.Discount.ToString("0.00");
                txtFinalAmount.Text = saleOrder.FinalAmount.ToString("C2");
                
                // 设置状态
                var statusItem = cmbStatus.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Tag.ToString() == saleOrder.Status);
                if (statusItem != null)
                {
                    cmbStatus.SelectedItem = statusItem;
                }
                
                // 填充销售明细
                _saleDetails.Clear();
                foreach (var detail in saleOrder.SaleDetails)
                {
                    _saleDetails.Add(detail);
                }
                
                // 等待UI更新，确保所有行都已创建
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    // 为所有销售明细行的下拉框设置数据源
                    UpdateAllRowComboBoxes();
                }), System.Windows.Threading.DispatcherPriority.Render);
            }
        }

        private void btnAddDetail_Click(object sender, RoutedEventArgs e)
        {
            var saleDetail = new SaleDetail
            {
                Quantity = 1,
                UnitPrice = 0.00m
            };
            _saleDetails.Add(saleDetail);
            
            // 等待UI更新，确保新行已创建
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // 获取新添加行的DataGridRow
                var row = dgSaleDetails.ItemContainerGenerator.ContainerFromItem(saleDetail) as DataGridRow;
                if (row != null)
                {
                    // 为新行的下拉框设置数据源
                    var cmbFurniture = FindVisualChild<ComboBox>(row, "cmbFurniture");
                    if (cmbFurniture != null)
                    {
                        cmbFurniture.ItemsSource = _furnitures ?? new List<Furniture>();
                        cmbFurniture.Tag = saleDetail;
                    }
                    
                    var cmbWarehouse = FindVisualChild<ComboBox>(row, "cmbWarehouse");
                    if (cmbWarehouse != null)
                    {
                        cmbWarehouse.ItemsSource = _warehouses ?? new List<Warehouse>();
                        cmbWarehouse.Tag = saleDetail;
                    }
                    
                    var txtQuantity = FindVisualChild<TextBox>(row, "txtQuantity");
                    if (txtQuantity != null)
                    {
                        txtQuantity.Tag = saleDetail;
                        txtQuantity.TextChanged += txtQuantity_TextChanged;
                    }
                }
            }), System.Windows.Threading.DispatcherPriority.Render);
            
            // 更新金额
            UpdateAmounts();
        }

        private void btnDeleteDetail_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var saleDetail = button?.Tag as SaleDetail;
            if (saleDetail != null)
            {
                _saleDetails.Remove(saleDetail);
                UpdateAmounts();
            }
        }

        private void txtQuantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 只允许输入数字
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void txtDiscount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 只允许输入数字和小数点
            if (!char.IsDigit(e.Text, 0) && e.Text != ".")
            {
                e.Handled = true;
            }
        }

        private void cmbFurniture_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                var saleDetail = comboBox.Tag as SaleDetail;
                if (saleDetail != null && comboBox.SelectedItem is Furniture selectedFurniture)
                {
                    saleDetail.FurnitureId = selectedFurniture.FurnitureId;
                    saleDetail.UnitPrice = selectedFurniture.SalePrice;
                    UpdateAmounts();
                }
            }
        }

        private void cmbWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                var saleDetail = comboBox.Tag as SaleDetail;
                if (saleDetail != null && comboBox.SelectedItem is Warehouse selectedWarehouse)
                {
                    saleDetail.WarehouseId = selectedWarehouse.WarehouseId;
                }
            }
        }

        private void dgSaleDetails_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            // 为每行的家具和仓库下拉框设置数据源
            if (e.Row.Item is SaleDetail saleDetail)
            {
                var cmbFurniture = FindVisualChild<ComboBox>(e.Row, "cmbFurniture");
                if (cmbFurniture != null)
                {
                    // 确保_furnitures不为null
                    var furnitureList = _furnitures ?? new List<Furniture>();
                    Console.WriteLine($"找到家具下拉框，家具数据数量: {furnitureList.Count}");
                    cmbFurniture.ItemsSource = furnitureList;
                    cmbFurniture.Tag = saleDetail;
                    if (saleDetail.FurnitureId > 0)
                    {
                        cmbFurniture.SelectedItem = furnitureList.FirstOrDefault(f => f.FurnitureId == saleDetail.FurnitureId);
                    }
                }
                else
                {
                    Console.WriteLine("未找到家具下拉框");
                }
                
                var cmbWarehouse = FindVisualChild<ComboBox>(e.Row, "cmbWarehouse");
                if (cmbWarehouse != null)
                {
                    // 确保_warehouses不为null
                    var warehouseList = _warehouses ?? new List<Warehouse>();
                    Console.WriteLine($"找到仓库下拉框，仓库数据数量: {warehouseList.Count}");
                    cmbWarehouse.ItemsSource = warehouseList;
                    cmbWarehouse.Tag = saleDetail;
                    if (saleDetail.WarehouseId > 0)
                    {
                        cmbWarehouse.SelectedItem = warehouseList.FirstOrDefault(w => w.WarehouseId == saleDetail.WarehouseId);
                    }
                }
                else
                {
                    Console.WriteLine("未找到仓库下拉框");
                }
                
                var txtQuantity = FindVisualChild<TextBox>(e.Row, "txtQuantity");
                if (txtQuantity != null)
                {
                    txtQuantity.Tag = saleDetail;
                    txtQuantity.TextChanged += txtQuantity_TextChanged;
                }
            }
        }

        private void txtQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                var saleDetail = textBox.Tag as SaleDetail;
                if (saleDetail != null)
                {
                    // 处理空字符串的情况
                    if (string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        saleDetail.Quantity = 0;
                        UpdateAmounts();
                        return;
                    }
                    
                    if (int.TryParse(textBox.Text, out int quantity))
                    {
                        saleDetail.Quantity = quantity;
                        UpdateAmounts();
                    }
                }
            }
        }

        private void txtDiscount_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAmounts();
        }

        private void UpdateAmounts()
        {
            // 计算总金额
            decimal totalAmount = _saleDetails.Sum(detail => detail.TotalPrice);
            txtTotalAmount.Text = totalAmount.ToString("C2");
            
            // 获取折扣
            decimal discount = 0.00m;
            if (decimal.TryParse(txtDiscount.Text, out decimal parsedDiscount))
            {
                discount = parsedDiscount;
            }
            
            // 计算最终金额
            decimal finalAmount = totalAmount - discount;
            txtFinalAmount.Text = finalAmount.ToString("C2");
        }

        #region Helper Methods
    
        /// <summary>
        /// 在Visual Tree中查找指定类型和名称的控件
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        /// <param name="parent">父控件</param>
        /// <param name="name">控件名称</param>
        /// <returns>找到的控件，如果没有找到则返回null</returns>
        private T? FindVisualChild<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                
                if (child is T frameworkElement && frameworkElement.Name == name)
                {
                    return frameworkElement;
                }
                
                T? result = FindVisualChild<T>(child, name);
                if (result != null)
                {
                    return result;
                }
            }
            
            return null;
        }
    
        #endregion

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // 验证输入
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                ShowError("请输入客户姓名");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(txtCustomerPhone.Text))
            {
                ShowError("请输入客户电话");
                return;
            }
            
            if (!dpSaleDate.SelectedDate.HasValue)
            {
                ShowError("请选择销售日期");
                return;
            }
            
            if (_saleDetails.Count == 0)
            {
                ShowError("请至少添加一条销售明细");
                return;
            }
            
            // 验证每条销售明细
            for (int i = 0; i < _saleDetails.Count; i++)
            {
                var detail = _saleDetails[i];
                if (detail.FurnitureId == 0)
                {
                    ShowError($"第 {i + 1} 行明细请选择家具");
                    return;
                }
                
                if (detail.WarehouseId == 0)
                {
                    ShowError($"第 {i + 1} 行明细请选择仓库");
                    return;
                }
                
                if (detail.Quantity <= 0)
                {
                    ShowError($"第 {i + 1} 行明细请输入有效的数量");
                    return;
                }
            }
            
            // 创建销售订单对象
            var saleOrder = new SaleOrder
            {
                SaleId = _saleOrderId,
                CustomerName = txtCustomerName.Text,
                CustomerPhone = txtCustomerPhone.Text,
                SaleDate = dpSaleDate.SelectedDate.Value,
                TotalAmount = _saleDetails.Sum(detail => detail.TotalPrice),
                Discount = decimal.TryParse(txtDiscount.Text, out decimal discount) ? discount : 0.00m,
                FinalAmount = _saleDetails.Sum(detail => detail.TotalPrice) - (decimal.TryParse(txtDiscount.Text, out decimal discount2) ? discount2 : 0.00m),
                Status = (cmbStatus.SelectedItem as ComboBoxItem)?.Tag.ToString() ?? "pending",
                CreatedBy = _currentUserId, // 设置创建人ID为当前登录用户ID
                SaleDetails = _saleDetails.ToList()
            };
            
            // 保存销售订单
            if (_saleOrderId > 0)
            {
                // 更新销售订单
                var response = await _apiService.UpdateSaleOrderAsync(_saleOrderId, saleOrder);
                if (response != null && response.Success)
                {
                    MessageBox.Show(response.Message);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    ShowError(response?.Message ?? "更新销售订单失败");
                }
            }
            else
            {
                // 创建销售订单
                var createdSaleOrder = await _apiService.CreateSaleOrderAsync(saleOrder);
                if (createdSaleOrder != null)
                {
                    MessageBox.Show("销售订单创建成功");
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    ShowError("创建销售订单失败");
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Visibility = Visibility.Visible;
        }
    }
}