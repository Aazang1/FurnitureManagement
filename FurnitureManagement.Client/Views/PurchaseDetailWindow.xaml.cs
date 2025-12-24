using System;
using System.Collections.Generic;
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
    /// PurchaseDetailWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PurchaseDetailWindow : Window
    {
        private readonly ApiService _apiService;
        private PurchaseOrder _purchaseOrder;

        public PurchaseDetailWindow(PurchaseOrder purchaseOrder)
        {
            InitializeComponent();
            _apiService = new ApiService();
            _purchaseOrder = purchaseOrder;
            LoadData();
        }

        // 加载采购订单明细数据
        private void LoadData()
        {
            if (_purchaseOrder != null)
            {
                txtOrderId.Text = _purchaseOrder.PurchaseOrderId.ToString();
                txtSupplier.Text = _purchaseOrder.Supplier?.SupplierName;
                txtPurchaseDate.Text = _purchaseOrder.PurchaseDate.ToString("yyyy-MM-dd");
                txtStatus.Text = _purchaseOrder.Status;
                txtTotalAmount.Text = _purchaseOrder.TotalAmount.ToString("C");
                dgPurchaseDetails.ItemsSource = _purchaseOrder.PurchaseDetails;
            }
        }

        // 关闭窗口
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}