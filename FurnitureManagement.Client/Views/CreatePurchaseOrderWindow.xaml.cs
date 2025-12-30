using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FurnitureManagement.Client.Models;
using FurnitureManagement.Client.Servcie;

namespace FurnitureManagement.Client.Views
{
    /// <summary>
    /// CreatePurchaseOrderWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CreatePurchaseOrderWindow : Window
    {
        private readonly ApiService _apiService;
        private List<Supplier>? _suppliers;
        private List<Furniture>? _furnitureList;
        private List<Warehouse>? _warehouseList;
        private List<PurchaseDetailViewModel> _detailViewModels = new List<PurchaseDetailViewModel>();

        public CreatePurchaseOrderWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
            dpPurchaseDate.SelectedDate = DateTime.Now;
            LoadDataAsync();
        }

        // 加载数据
        private async void LoadDataAsync()
        {
            await LoadSuppliersAsync();
            await LoadFurnitureAsync();
            await LoadWarehouseAsync();
        }

        // 加载供应商列表
        private async Task LoadSuppliersAsync()
        {
            _suppliers = await _apiService.GetSuppliersAsync();
            if (_suppliers != null)
            {
                cmbSupplier.ItemsSource = _suppliers;
                cmbSupplier.DisplayMemberPath = "SupplierName";
                cmbSupplier.SelectedValuePath = "SupplierId";
            }
        }

        // 加载商品列表
        private async Task LoadFurnitureAsync()
        {
            _furnitureList = await _apiService.GetFurnitureAsync();
        }

        // 加载仓库列表
        private async Task LoadWarehouseAsync()
        {
            _warehouseList = await _apiService.GetWarehousesAsync();
        }

        // 添加采购明细
        private void btnAddDetail_Click(object sender, RoutedEventArgs e)
        {
            // 检查数据是否加载完成
            if (_furnitureList == null || _furnitureList.Count == 0)
            {
                MessageBox.Show("商品数据加载中，请稍后再试", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_warehouseList == null || _warehouseList.Count == 0)
            {
                MessageBox.Show("仓库数据加载中，请稍后再试", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var detailViewModel = new PurchaseDetailViewModel()
            {
                FurnitureList = _furnitureList,
                WarehouseList = _warehouseList,
                Quantity = 1,
                UnitPrice = 0
            };

            // 添加属性更改事件监听，以便实时更新总金额
            detailViewModel.PropertyChanged += DetailViewModel_PropertyChanged;

            _detailViewModels.Add(detailViewModel);
            dgPurchaseDetails.ItemsSource = null;
            dgPurchaseDetails.ItemsSource = _detailViewModels;

            UpdateTotalAmount();
        }

        // 删除采购明细
        private void btnDeleteDetail_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var viewModel = button?.Tag as PurchaseDetailViewModel;
            if (viewModel != null)
            {
                viewModel.PropertyChanged -= DetailViewModel_PropertyChanged;
                _detailViewModels.Remove(viewModel);
                dgPurchaseDetails.ItemsSource = null;
                dgPurchaseDetails.ItemsSource = _detailViewModels;
                UpdateTotalAmount();
            }
        }

        // 明细属性更改事件处理
        private void DetailViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            UpdateTotalAmount();
        }

        // 更新总金额
        private void UpdateTotalAmount()
        {
            decimal total = _detailViewModels.Sum(detail => detail.Amount);
            txtTotalAmount.Text = total.ToString("C");
        }

        // 保存采购订单
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // 验证输入
            if (cmbSupplier.SelectedItem == null)
            {
                MessageBox.Show("请选择供应商", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (dpPurchaseDate.SelectedDate == null)
            {
                MessageBox.Show("请选择采购日期", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_detailViewModels.Count == 0)
            {
                MessageBox.Show("请至少添加一条采购明细", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 验证每条明细的必填项
            for (int i = 0; i < _detailViewModels.Count; i++)
            {
                var detail = _detailViewModels[i];
                
                // 调试信息：输出明细状态
                Console.WriteLine($"第{i + 1}条明细状态 - 商品: {detail.Furniture?.FurnitureName ?? "null"}, 仓库: {detail.Warehouse?.WarehouseName ?? "null"}, 数量: {detail.Quantity}");
                
                if (detail.Furniture == null)
                {
                    MessageBox.Show($"第{i + 1}条明细：请选择商品", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (detail.Warehouse == null)
                {
                    MessageBox.Show($"第{i + 1}条明细：请选择仓库", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (detail.Quantity <= 0)
                {
                    MessageBox.Show($"第{i + 1}条明细：请输入有效的采购数量", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            // 创建采购订单
            var order = new PurchaseOrder
            {
                SupplierId = (int)cmbSupplier.SelectedValue,
                PurchaseDate = dpPurchaseDate.SelectedDate.Value,
                PurchaseDetails = new List<PurchaseDetail>()
            };

            // 添加采购明细
            foreach (var detail in _detailViewModels)
            {
                order.PurchaseDetails.Add(new PurchaseDetail
                {
                    FurnitureId = detail.Furniture.FurnitureId,
                    WarehouseId = detail.Warehouse.WarehouseId,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.UnitPrice
                });
            }

            // 保存采购订单
            var result = await _apiService.CreatePurchaseOrderAsync(order);
            if (result != null)
            {
                MessageBox.Show("采购订单创建成功", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("采购订单创建失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 取消
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        // 商品选择变更事件处理
        private void cmbFurniture_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var currentRow = comboBox.DataContext as PurchaseDetailViewModel;
                if (currentRow != null)
                {
                    var selectedFurniture = comboBox.SelectedItem as Furniture;
                    if (selectedFurniture != null)
                    {
                        // 设置单价为进货价格
                        currentRow.UnitPrice = selectedFurniture.PurchasePrice;
                        
                        // 更新金额属性以触发UI更新
                        currentRow.OnPropertyChanged(nameof(currentRow.Amount));
                        
                        // 更新总金额
                        UpdateTotalAmount();
                        
                        Console.WriteLine($"选择商品: {selectedFurniture.FurnitureName}, 设置单价: {selectedFurniture.PurchasePrice}");
                    }
                }
            }
        }
    }

    // 采购明细视图模型
    public class PurchaseDetailViewModel : INotifyPropertyChanged
    {
        private Furniture? _furniture;
        private Warehouse? _warehouse;
        private int _quantity;
        private decimal _unitPrice;

        public List<Furniture>? FurnitureList { get; set; }
        public List<Warehouse>? WarehouseList { get; set; }

        public Furniture? Furniture
        {
            get => _furniture;
            set
            {
                _furniture = value;
                OnPropertyChanged(nameof(Furniture));
                
                // 当选择商品时，自动设置单价为该商品的进货价格
                if (value != null)
                {
                    UnitPrice = value.PurchasePrice;
                }
                else
                {
                    UnitPrice = 0;
                }
                
                OnPropertyChanged(nameof(Amount));
            }
        }

        public Warehouse? Warehouse
        {
            get => _warehouse;
            set
            {
                _warehouse = value;
                OnPropertyChanged(nameof(Warehouse));
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(Amount));
            }
        }

        public decimal UnitPrice
        {
            get => _unitPrice;
            set
            {
                _unitPrice = value;
                OnPropertyChanged(nameof(UnitPrice));
                OnPropertyChanged(nameof(Amount));
            }
        }

        public decimal Amount => Quantity * UnitPrice;

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}