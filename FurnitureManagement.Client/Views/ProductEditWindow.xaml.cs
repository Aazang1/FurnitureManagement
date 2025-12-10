using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using FurnitureManagement.Client.Models;
using FurnitureManagement.Client.Servcie;

namespace FurnitureManagement.Client.Views
{
    /// <summary>
    /// ProductEditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProductEditWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly Furniture? _product;
        private readonly List<Category>? _categories;
        private bool _isEditMode;

        public ProductEditWindow(Furniture? product, List<Category>? categories)
        {
            InitializeComponent();
            _apiService = new ApiService();
            _product = product;
            _categories = categories;
            _isEditMode = product != null;
            
            InitializeUI();
        }

        // 初始化UI
        private void InitializeUI()
        {
            // 设置标题
            txtTitle.Text = _isEditMode ? "编辑商品" : "添加商品";
            
            // 加载分类数据
            if (_categories != null && _categories.Count > 0)
            {
                cmbCategory.ItemsSource = _categories;
            }
            
            // 如果是编辑模式，填充表单数据
            if (_isEditMode && _product != null)
            {
                txtFurnitureName.Text = _product.FurnitureName;
                cmbCategory.SelectedValue = _product.CategoryId;
                txtPrice.Text = _product.Price.ToString();
                txtDescription.Text = _product.Description ?? string.Empty;
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
                // 创建或更新商品对象
                var product = _isEditMode ? _product : new Furniture();
                
                if (product != null)
                {
                    product.FurnitureName = txtFurnitureName.Text.Trim();
                    product.CategoryId = (int)cmbCategory.SelectedValue;
                    product.Price = decimal.Parse(txtPrice.Text.Trim());
                    product.Description = txtDescription.Text.Trim();
                    
                    if (_isEditMode)
                    {
                        // 更新商品
                        var response = await _apiService.UpdateFurnitureAsync(product.FurnitureId, product);
                        if (response != null && response.Success)
                        {
                            MessageBox.Show("商品更新成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.DialogResult = true;
                            this.Close();
                        }
                        else
                        {
                            ShowError(response?.Message ?? "更新商品失败");
                        }
                    }
                    else
                    {
                        // 创建商品
                        var createdProduct = await _apiService.CreateFurnitureAsync(product);
                        if (createdProduct != null)
                        {
                            MessageBox.Show("商品添加成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.DialogResult = true;
                            this.Close();
                        }
                        else
                        {
                            ShowError("添加商品失败");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("保存商品失败，请重试");
                Console.WriteLine($"保存商品失败: {ex.Message}");
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
            
            // 验证商品名称
            if (string.IsNullOrWhiteSpace(txtFurnitureName.Text))
            {
                ShowError("请输入商品名称");
                return false;
            }
            
            // 验证分类选择
            if (cmbCategory.SelectedValue == null)
            {
                ShowError("请选择商品分类");
                return false;
            }
            
            // 验证价格
            if (string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                ShowError("请输入商品价格");
                return false;
            }
            
            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                ShowError("请输入有效的商品价格");
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

        // 价格输入验证，只允许输入数字和小数点
        private void txtPrice_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]*[.,]?[0-9]*$");
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}