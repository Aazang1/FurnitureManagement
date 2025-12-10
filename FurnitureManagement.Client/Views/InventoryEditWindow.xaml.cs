using System.Text.RegularExpressions;
using System.Windows;
using FurnitureManagement.Client.Models;
using FurnitureManagement.Client.Servcie;

namespace FurnitureManagement.Client.Views
{
    /// <summary>
    /// InventoryEditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InventoryEditWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly Inventory? _inventory;
        private readonly List<Furniture>? _products;
        private readonly List<Warehouse>? _warehouses;
        private bool _isEditMode;

        public InventoryEditWindow(Inventory? inventory, List<Furniture>? products, List<Warehouse>? warehouses)
        {
            InitializeComponent();
            _apiService = new ApiService();
            _inventory = inventory;
            _products = products;
            _warehouses = warehouses;
            _isEditMode = inventory != null;
            
            InitializeUI();
        }

        // 初始化UI
        private void InitializeUI()
        {
            // 设置标题
            txtTitle.Text = _isEditMode ? "编辑库存" : "添加库存";
            
            // 加载商品数据
            if (_products != null && _products.Count > 0)
            {
                cmbFurniture.ItemsSource = _products;
            }
            
            // 加载仓库数据
            if (_warehouses != null && _warehouses.Count > 0)
            {
                cmbWarehouse.ItemsSource = _warehouses;
            }
            
            // 如果是编辑模式，填充表单数据
            if (_isEditMode && _inventory != null)
            {
                cmbFurniture.SelectedValue = _inventory.FurnitureId;
                cmbWarehouse.SelectedValue = _inventory.WarehouseId;
                txtQuantity.Text = _inventory.Quantity.ToString();
            }
        }

        // 保存按钮点击事件
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // 验证表单
            if (!ValidateForm())
                return;

            try
            {
                // 创建或更新库存对象
                var inventory = _isEditMode ? _inventory : new Inventory();
                
                if (inventory != null)
                {
                    inventory.FurnitureId = (int)cmbFurniture.SelectedValue;
                    inventory.WarehouseId = (int)cmbWarehouse.SelectedValue;
                    inventory.Quantity = int.Parse(txtQuantity.Text.Trim());
                    
                    if (_isEditMode)
                    {
                        // 更新库存
                        var response = await _apiService.UpdateInventoryAsync(inventory.InventoryId, inventory);
                        if (response != null && response.Success)
                        {
                            MessageBox.Show("库存更新成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.DialogResult = true;
                            this.Close();
                        }
                        else
                        {
                            ShowError(response?.Message ?? "更新库存失败");
                        }
                    }
                    else
                    {
                        // 创建库存
                        var createdInventory = await _apiService.CreateInventoryAsync(inventory);
                        if (createdInventory != null)
                        {
                            MessageBox.Show("库存添加成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.DialogResult = true;
                            this.Close();
                        }
                        else
                        {
                            ShowError("添加库存失败");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("保存库存失败，请重试");
                Console.WriteLine($"保存库存失败: {ex.Message}");
            }
        }

        // 取消按钮点击事件
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        // 验证表单
        private bool ValidateForm()
        {
            // 清除之前的错误信息
            txtError.Visibility = Visibility.Collapsed;
            txtError.Text = string.Empty;
            
            // 验证商品选择
            if (cmbFurniture.SelectedValue == null)
            {
                ShowError("请选择商品");
                return false;
            }
            
            // 验证仓库选择
            if (cmbWarehouse.SelectedValue == null)
            {
                ShowError("请选择仓库");
                return false;
            }
            
            // 验证数量
            if (string.IsNullOrWhiteSpace(txtQuantity.Text))
            {
                ShowError("请输入数量");
                return false;
            }
            
            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
            {
                ShowError("请输入有效的数量");
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

        // 数量输入验证，只允许输入数字
        private void txtQuantity_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]*$");
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}