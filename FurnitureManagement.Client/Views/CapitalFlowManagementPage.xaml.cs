using FurnitureManagement.Client.Models;
using FurnitureManagement.Client.Servcie;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace FurnitureManagement.Client.Views
{
    public partial class CapitalFlowManagementPage : Page, INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private ObservableCollection<CapitalFlow> _capitalFlows;
        private CapitalFlowSummary _capitalSummary;

        // 属性
        public ObservableCollection<CapitalFlow> CapitalFlows
        {
            get { return _capitalFlows; }
            set
            {
                _capitalFlows = value;
                OnPropertyChanged(nameof(CapitalFlows));
            }
        }

        public CapitalFlowSummary CapitalSummary
        {
            get { return _capitalSummary; }
            set
            {
                _capitalSummary = value;
                OnPropertyChanged(nameof(CapitalSummary));
            }
        }

        // INotifyPropertyChanged 实现
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public CapitalFlowManagementPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            CapitalFlows = new ObservableCollection<CapitalFlow>();
            CapitalSummary = new CapitalFlowSummary();
            DataContext = this;
            Task.Run(async () => await LoadData());
        }

        /// <summary>
        /// 加载所有数据
        /// </summary>
        private async Task LoadData()
        {
            await LoadCapitalFlowsAsync();
            await LoadCapitalSummaryAsync();
        }

        /// <summary>
        /// 加载资金流水数据
        /// </summary>
        private async Task LoadCapitalFlowsAsync()
        {
            try
            {
                var capitalFlows = await _apiService.GetCapitalFlowsAsync();
                if (capitalFlows != null)
                {
                    // 使用Dispatcher确保在UI线程上更新集合
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        CapitalFlows.Clear();
                        foreach (var item in capitalFlows)
                        {
                            CapitalFlows.Add(item);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                // 使用Dispatcher确保在UI线程上显示消息框
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"加载资金流水数据失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        /// <summary>
        /// 加载资金汇总数据
        /// </summary>
        private async Task LoadCapitalSummaryAsync()
        {
            try
            {
                var summary = await _apiService.GetCapitalFlowSummaryAsync();
                if (summary != null)
                {
                    // 使用Dispatcher确保在UI线程上更新属性
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        CapitalSummary = summary;
                    });
                }
            }
            catch (Exception ex)
            {
                // 使用Dispatcher确保在UI线程上显示消息框
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"加载资金汇总数据失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }
    }
}