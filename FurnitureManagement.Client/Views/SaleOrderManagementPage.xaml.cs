using FurnitureManagement.Client.Models;
using FurnitureManagement.Client.Servcie;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace FurnitureManagement.Client.Views
{
    /// <summary>
    /// SaleOrderManagementPage.xaml 的交互逻辑
    /// </summary>
    public partial class SaleOrderManagementPage : Page
    {
        private readonly ApiService _apiService;
        private ObservableCollection<SaleOrder> _saleOrders;
        private int _currentUserId;

        public SaleOrderManagementPage(int currentUserId = 0)
        {
            InitializeComponent();
            _apiService = new ApiService();
            _saleOrders = new ObservableCollection<SaleOrder>();
            dgSaleOrders.ItemsSource = _saleOrders;
            _currentUserId = currentUserId;
            
            // 设置状态下拉框默认选中"全部"选项
            cmbStatus.SelectedIndex = 0;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var selectedStatus = (cmbStatus.SelectedItem as ComboBoxItem)?.Tag.ToString();
            await LoadSaleOrders(null, null, selectedStatus, null);
        }

        private async Task LoadSaleOrders(DateTime? startDate = null, DateTime? endDate = null, string? status = null, string? search = null)
        {
            var saleOrders = await _apiService.GetSaleOrdersAsync(startDate, endDate, status, search);
            if (saleOrders != null)
            {
                _saleOrders.Clear();
                foreach (var saleOrder in saleOrders)
                {
                    _saleOrders.Add(saleOrder);
                }
            }
        }

        private async void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            var selectedStatus = (cmbStatus.SelectedItem as ComboBoxItem)?.Tag.ToString();
            await LoadSaleOrders(dpStartDate.SelectedDate, dpEndDate.SelectedDate, selectedStatus, txtSearch.Text);
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var selectedStatus = (cmbStatus.SelectedItem as ComboBoxItem)?.Tag.ToString();
            await LoadSaleOrders(dpStartDate.SelectedDate, dpEndDate.SelectedDate, selectedStatus, txtSearch.Text);
        }

        private async void btnReset_Click(object sender, RoutedEventArgs e)
        {
            dpStartDate.SelectedDate = null;
            dpEndDate.SelectedDate = null;
            cmbStatus.SelectedIndex = 0;
            txtSearch.Text = string.Empty;
            await LoadSaleOrders();
        }

        private void btnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            var saleOrderEditWindow = new SaleOrderEditWindow(currentUserId: _currentUserId);
            saleOrderEditWindow.Closed += async (s, args) =>
            {
                if (saleOrderEditWindow.DialogResult == true)
                {
                    await LoadSaleOrders();
                }
            };
            saleOrderEditWindow.ShowDialog();
        }

        private void btnViewOrder_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var saleOrder = button?.Tag as SaleOrder;
            if (saleOrder != null)
            {
                var saleOrderEditWindow = new SaleOrderEditWindow(saleOrder.SaleId, isViewMode: true);
                saleOrderEditWindow.ShowDialog();
            }
        }

        private void btnEditOrder_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var saleOrder = button?.Tag as SaleOrder;
            if (saleOrder != null)
            {
                var saleOrderEditWindow = new SaleOrderEditWindow(saleOrder.SaleId);
                saleOrderEditWindow.Closed += async (s, args) =>
                {
                    if (saleOrderEditWindow.DialogResult == true)
                    {
                        await LoadSaleOrders();
                    }
                };
                saleOrderEditWindow.ShowDialog();
            }
        }

        private async void btnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var saleOrder = button?.Tag as SaleOrder;
            if (saleOrder != null)
            {
                var result = MessageBox.Show($"确定要删除订单 {saleOrder.SaleId} 吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var response = await _apiService.DeleteSaleOrderAsync(saleOrder.SaleId);
                    if (response != null && response.Success)
                    {
                        MessageBox.Show(response.Message);
                        await LoadSaleOrders();
                    }
                    else
                    {
                        MessageBox.Show(response?.Message ?? "删除失败");
                    }
                }
            }
        }
    }
}