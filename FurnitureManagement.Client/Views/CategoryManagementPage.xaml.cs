using System;using System.Collections.Generic;using System.Linq;using System.Text;using System.Threading.Tasks;using System.Windows;using System.Windows.Controls;using System.Windows.Data;using System.Windows.Documents;using System.Windows.Input;using System.Windows.Media;using System.Windows.Media.Imaging;using System.Windows.Navigation;using System.Windows.Shapes;
using FurnitureManagement.Client.Servcie;
using FurnitureManagement.Client.Models;

namespace FurnitureManagement.Client.Views
{
    /// <summary>
    /// CategoryManagementPage.xaml 的交互逻辑
    /// </summary>
    public partial class CategoryManagementPage : Page
    {
        private readonly ApiService _apiService;
        private List<Category>? _allCategories;

        public CategoryManagementPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            // 页面加载时加载分类列表
            LoadCategoriesAsync();
        }

        // 加载分类列表
        private async void LoadCategoriesAsync()
        {
            try
            {
                _allCategories = await _apiService.GetCategoriesAsync();
                dgCategories.ItemsSource = _allCategories;
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载分类列表失败，请重试", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"加载分类失败: {ex.Message}");
            }
        }

        private void btnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            // 添加分类逻辑
            var editWindow = new CategoryEditWindow();
            if (editWindow.ShowDialog() == true)
            {
                // 刷新分类列表
                LoadCategoriesAsync();
            }
        }

        private void btnEditCategory_Click(object sender, RoutedEventArgs e)
        {
            // 编辑分类逻辑
            var button = sender as Button;
            var category = button?.Tag as Category;
            if (category != null)
            {
                var editWindow = new CategoryEditWindow(category);
                if (editWindow.ShowDialog() == true)
                {
                    // 刷新分类列表
                    LoadCategoriesAsync();
                }
            }
        }

        private async void btnDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            // 删除分类逻辑
            var button = sender as Button;
            var category = button?.Tag as Category;
            if (category != null)
            {
                var result = MessageBox.Show($"确定要删除分类 {category.CategoryName} 吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _apiService.DeleteCategoryAsync(category.CategoryId);
                        MessageBox.Show("分类删除成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadCategoriesAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("删除分类失败，请重试", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        Console.WriteLine($"删除分类异常: {ex.Message}");
                    }
                }
            }
        }
    }
}