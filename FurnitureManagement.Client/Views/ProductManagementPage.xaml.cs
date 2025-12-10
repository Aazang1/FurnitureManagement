using System.Windows;
using System.Windows.Controls;
using FurnitureManagement.Client.Models;
using FurnitureManagement.Client.Servcie;

namespace FurnitureManagement.Client.Views
{
    /// <summary>
    /// ProductManagementPage.xaml 的交互逻辑
    /// </summary>
    public partial class ProductManagementPage : Page
    {
        private readonly ApiService _apiService;
        private List<Furniture>? _allProducts;
        private List<Category>? _allCategories;

        public ProductManagementPage()
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
                // 并行加载商品和分类数据
                var productsTask = _apiService.GetFurnitureAsync();
                var categoriesTask = _apiService.GetCategoriesAsync();
                
                await Task.WhenAll(productsTask, categoriesTask);
                
                _allProducts = await productsTask;
                _allCategories = await categoriesTask;
                
                if (_allProducts != null)
                {
                    // 关联商品和分类
                    foreach (var product in _allProducts)
                    {
                        if (_allCategories != null)
                        {
                            product.Category = _allCategories.FirstOrDefault(c => c.CategoryId == product.CategoryId);
                        }
                    }
                    dgProducts.ItemsSource = _allProducts;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载数据失败，请重试", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"加载商品数据失败: {ex.Message}");
            }
        }

        // 搜索按钮点击事件
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (_allProducts == null)
                return;

            string searchText = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                dgProducts.ItemsSource = _allProducts;
            }
            else
            {
                var filteredProducts = _allProducts.Where(p => p.FurnitureName.ToLower().Contains(searchText)).ToList();
                dgProducts.ItemsSource = filteredProducts;
            }
        }

        // 添加商品按钮点击事件
        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            // 打开商品编辑窗口（新建商品）
            var editWindow = new ProductEditWindow(null, _allCategories);
            editWindow.Closed += (s, args) =>
            {
                // 窗口关闭后重新加载数据
                LoadDataAsync();
            };
            editWindow.ShowDialog();
        }

        // 编辑商品按钮点击事件
        private void btnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button?.Tag as Furniture;
            if (product != null)
            {
                // 打开商品编辑窗口（编辑现有商品）
                var editWindow = new ProductEditWindow(product, _allCategories);
                editWindow.Closed += (s, args) =>
                {
                    // 窗口关闭后重新加载数据
                    LoadDataAsync();
                };
                editWindow.ShowDialog();
            }
        }

        // 删除商品按钮点击事件
        private async void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button?.Tag as Furniture;
            if (product != null)
            {
                // 显示确认对话框
                var result = MessageBox.Show($"确定要删除商品 '{product.FurnitureName}' 吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var response = await _apiService.DeleteFurnitureAsync(product.FurnitureId);
                        if (response != null && response.Success)
                        {
                            MessageBox.Show("商品删除成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadDataAsync();
                        }
                        else
                        {
                            MessageBox.Show(response?.Message ?? "删除失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("删除商品失败，请重试", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        Console.WriteLine($"删除商品异常: {ex.Message}");
                    }
                }
            }
        }
    }
}