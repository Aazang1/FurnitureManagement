using FurnitureManagement.Client.Models;
using FurnitureManagement.Client.Servcie;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace FurnitureManagement.Client.Views
{
    public partial class ReportManagementPage : Page
    {
        private readonly ApiService _apiService;
        private ObservableCollection<InventorySummary> _inventorySummary;
        private ObservableCollection<SalesDaily> _salesDaily;
        private ObservableCollection<UserOperations> _userOperations;

        public ReportManagementPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            InitializeCollections();
            LoadData();
        }

        /// <summary>
        /// 初始化集合
        /// </summary>
        private void InitializeCollections()
        {
            _inventorySummary = new ObservableCollection<InventorySummary>();
            _salesDaily = new ObservableCollection<SalesDaily>();
            _userOperations = new ObservableCollection<UserOperations>();

            dgInventorySummary.ItemsSource = _inventorySummary;
            dgSalesDaily.ItemsSource = _salesDaily;
            dgUserOperations.ItemsSource = _userOperations;
        }

        /// <summary>
        /// 加载所有报表数据
        /// </summary>
        private async void LoadData()
        {
            await LoadInventorySummaryAsync();
            await LoadSalesDailyAsync();
            await LoadUserOperationsAsync();
        }

        /// <summary>
        /// 加载库存管理报表数据
        /// </summary>
        private async Task LoadInventorySummaryAsync()
        {
            try
            {
                _inventorySummary.Clear();
                var inventorySummary = await _apiService.GetInventorySummaryAsync();
                if (inventorySummary != null)
                {
                    foreach (var item in inventorySummary)
                    {
                        _inventorySummary.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载库存管理报表失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 加载销售日报数据
        /// </summary>
        private async Task LoadSalesDailyAsync()
        {
            try
            {
                _salesDaily.Clear();
                var salesDaily = await _apiService.GetSalesDailyAsync();
                if (salesDaily != null)
                {
                    foreach (var item in salesDaily)
                    {
                        _salesDaily.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载销售日报失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 加载人员操作报表数据
        /// </summary>
        private async Task LoadUserOperationsAsync()
        {
            try
            {
                _userOperations.Clear();
                var userOperations = await _apiService.GetUserOperationsAsync();
                if (userOperations != null)
                {
                    foreach (var item in userOperations)
                    {
                        _userOperations.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载人员操作报表失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 刷新库存管理报表
        /// </summary>
        private async void btnRefreshInventorySummary_Click(object sender, RoutedEventArgs e)
        {
            await LoadInventorySummaryAsync();
        }

        /// <summary>
        /// 刷新销售日报
        /// </summary>
        private async void btnRefreshSalesDaily_Click(object sender, RoutedEventArgs e)
        {
            await LoadSalesDailyAsync();
        }

        /// <summary>
        /// 刷新人员操作报表
        /// </summary>
        private async void btnRefreshUserOperations_Click(object sender, RoutedEventArgs e)
        {
            await LoadUserOperationsAsync();
        }
    }
}