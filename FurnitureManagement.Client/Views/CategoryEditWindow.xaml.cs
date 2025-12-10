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
using System.Windows.Navigation;
using System.Windows.Shapes;
using FurnitureManagement.Client.Models;
using FurnitureManagement.Client.Servcie;

namespace FurnitureManagement.Client.Views
{
    /// <summary>
    /// CategoryEditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CategoryEditWindow : Window
    {
        private readonly ApiService _apiService;
        private Category? _category;

        // 用于添加新分类
        public CategoryEditWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
            txtTitle.Text = "添加分类";
        }

        // 用于编辑现有分类
        public CategoryEditWindow(Category category)
        {
            InitializeComponent();
            _apiService = new ApiService();
            _category = category;
            txtTitle.Text = "编辑分类";
            LoadCategoryData();
        }

        // 加载分类数据到表单
        private void LoadCategoryData()
        {
            if (_category != null)
            {
                txtCategoryName.Text = _category.CategoryName;
                txtDescription.Text = _category.Description;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // 取消逻辑
            this.DialogResult = false;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // 验证输入
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                txtError.Text = "分类名称不能为空";
                txtError.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                if (_category == null)
                {
                    // 添加新分类
                    var newCategory = new Category
                    {
                        CategoryName = txtCategoryName.Text.Trim(),
                        Description = txtDescription.Text.Trim()
                    };

                    var result = await _apiService.CreateCategoryAsync(newCategory);
                    if (result != null)
                    {
                        MessageBox.Show("分类添加成功", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                    }
                    else
                    {
                        txtError.Text = "添加分类失败";
                        txtError.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    // 更新现有分类
                    _category.CategoryName = txtCategoryName.Text.Trim();
                    _category.Description = txtDescription.Text.Trim();
                    _category.UpdatedAt = DateTime.Now;

                    var result = await _apiService.UpdateCategoryAsync(_category.CategoryId, _category);
                    if (result != null && result.Success)
                    {
                        MessageBox.Show("分类更新成功", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                    }
                    else
                    {
                        txtError.Text = result?.Message ?? "更新分类失败";
                        txtError.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                txtError.Text = "操作失败：" + ex.Message;
                txtError.Visibility = Visibility.Visible;
                Console.WriteLine($"分类操作失败: {ex.Message}");
            }
        }
    }
}