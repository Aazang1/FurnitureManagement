using FurnitureManagement.Client.Models;
using FurnitureManagement.Client.Servcie;
using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace FurnitureManagement.Client.Views
{
    public partial class ReportManagementPage : Page, INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private ObservableCollection<InventorySummary> _inventorySummary;
        private ObservableCollection<InventorySummary> _inventoryByWarehouse;
        private ObservableCollection<SalesDaily> _salesDaily;
        private ObservableCollection<UserOperations> _userOperations;
        private SeriesCollection _inventoryChartSeries;
        private SeriesCollection _salesChartSeries;
        private bool _showSummaryView = true; // 默认显示汇总视图

        // 图表数据属性
        public SeriesCollection InventoryChartSeries
        {
            get { return _inventoryChartSeries; }
            set
            {
                _inventoryChartSeries = value;
                OnPropertyChanged(nameof(InventoryChartSeries));
            }
        }

        public SeriesCollection SalesChartSeries
        {
            get { return _salesChartSeries; }
            set
            {
                _salesChartSeries = value;
                OnPropertyChanged(nameof(SalesChartSeries));
            }
        }

        // 视图切换属性
        public bool ShowSummaryView
        {
            get { return _showSummaryView; }
            set
            {
                _showSummaryView = value;
                OnPropertyChanged(nameof(ShowSummaryView));
                OnPropertyChanged(nameof(CurrentInventoryData));
            }
        }

        // 当前库存数据（根据视图模式切换）
        public ObservableCollection<InventorySummary> CurrentInventoryData => ShowSummaryView ? _inventorySummary : _inventoryByWarehouse;
        
        /// <summary>
        /// 图表X轴标签（商品名称列表）
        /// </summary>
        public ObservableCollection<string> InventoryFurnitureNames { get; } = new ObservableCollection<string>();

        /// <summary>
        /// 图表X轴标签（销售日期列表）
        /// </summary>
        public ObservableCollection<string> SalesDates { get; } = new ObservableCollection<string>();

        // INotifyPropertyChanged 实现
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ReportManagementPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            InitializeCollections();
            DataContext = this; // 设置数据上下文
            LoadData();
        }

        /// <summary>
        /// 初始化集合
        /// </summary>
        private void InitializeCollections()
        {
            _inventorySummary = new ObservableCollection<InventorySummary>();
            _inventoryByWarehouse = new ObservableCollection<InventorySummary>();
            _salesDaily = new ObservableCollection<SalesDaily>();
            _userOperations = new ObservableCollection<UserOperations>();
            InventoryChartSeries = new SeriesCollection();
            SalesChartSeries = new SeriesCollection();

            dgUserOperations.ItemsSource = _userOperations;
        }

        /// <summary>
        /// 加载所有报表数据
        /// </summary>
        private async void LoadData()
        {
            await LoadInventorySummaryAsync();
            await LoadInventoryByWarehouseAsync();
            await LoadSalesDailyAsync();
            await LoadUserOperationsAsync();
        }

        /// <summary>
        /// 加载库存管理报表数据（汇总视图）
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
                    // 如果当前是汇总视图，重新生成图表
                    if (ShowSummaryView)
                    {
                        GenerateInventoryChart();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载库存管理报表失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 加载库存管理报表数据（按仓库分类）
        /// </summary>
        private async Task LoadInventoryByWarehouseAsync()
        {
            try
            {
                _inventoryByWarehouse.Clear();
                var inventoryByWarehouse = await _apiService.GetInventoryByWarehouseAsync();
                if (inventoryByWarehouse != null)
                {
                    foreach (var item in inventoryByWarehouse)
                    {
                        _inventoryByWarehouse.Add(item);
                    }
                    // 如果当前是详细视图，重新生成图表
                    if (!ShowSummaryView)
                    {
                        GenerateInventoryChart();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载按仓库分类的库存数据失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    // 生成销售图表数据
                    GenerateSalesChart();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载销售日报失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 生成库存数量分布图表
        /// </summary>
        private void GenerateInventoryChart()
        {
            InventoryChartSeries.Clear();
            InventoryFurnitureNames.Clear();
            
            // 添加数据 - 创建对应的furniture_name标签
            var quantityValues = new ChartValues<double>();
            
            foreach (var item in CurrentInventoryData.Take(10)) // 只显示前10个商品，避免图表过于拥挤
            {
                quantityValues.Add((double)item.Quantity);
                InventoryFurnitureNames.Add(item.FurnitureName);
            }
            
            // 创建库存数量柱状图
            var quantitySeries = new ColumnSeries
            {
                Title = ShowSummaryView ? "库存数量（汇总）" : "库存数量（按仓库）",
                Values = quantityValues,
                DataLabels = true,
                LabelPoint = point => $"{point.Y}",
                Fill = System.Windows.Media.Brushes.CornflowerBlue
            };
            
            InventoryChartSeries.Add(quantitySeries);
        }

        /// <summary>
        /// 生成销售趋势图表
        /// </summary>
        private void GenerateSalesChart()
        {
            SalesChartSeries.Clear();
            SalesDates.Clear();
            
            // 创建销售总额折线图
            var salesSeries = new LineSeries
            {
                Title = "销售总额",
                Values = new ChartValues<double>(),
                DataLabels = true,
                LabelPoint = point => $"{point.Y:C2}",
                Stroke = System.Windows.Media.Brushes.DarkGreen,
                Fill = System.Windows.Media.Brushes.Transparent,
                LineSmoothness = 0.5
            };
            
            // 添加数据 - 将decimal转换为double，同时收集销售日期
            foreach (var item in _salesDaily.OrderBy(s => s.SaleDay))
            {
                salesSeries.Values.Add(Convert.ToDouble(item.TotalSales));
                SalesDates.Add(item.SaleDay.ToString("yyyy-MM-dd"));
            }
            
            SalesChartSeries.Add(salesSeries);
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
        /// 视图切换按钮点击事件
        /// </summary>
        private void btnSwitchView_Click(object sender, RoutedEventArgs e)
        {
            ShowSummaryView = !ShowSummaryView;
            
            // 重新生成图表以反映当前视图的数据
            GenerateInventoryChart();
        }

        /// <summary>
        /// 刷新库存管理报表
        /// </summary>
        private async void btnRefreshInventorySummary_Click(object sender, RoutedEventArgs e)
        {
            await LoadInventorySummaryAsync();
            await LoadInventoryByWarehouseAsync();
            GenerateInventoryChart();
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