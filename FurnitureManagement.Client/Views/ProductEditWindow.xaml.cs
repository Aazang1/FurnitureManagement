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
        private readonly int? _currentUserId;
        private bool _isEditMode;

        public ProductEditWindow(Furniture? product, List<Category>? categories, int? currentUserId = null)
        {
            InitializeComponent();
            _apiService = new ApiService();
            _product = product;
            _categories = categories;
            _currentUserId = currentUserId;
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
                txtModel.Text = _product.Model ?? string.Empty;
                txtPurchasePrice.Text = _product.PurchasePrice.ToString("F2");
                txtSalePrice.Text = _product.SalePrice.ToString("F2");
                txtDescription.Text = _product.Description ?? string.Empty;
            }
        }

        // 保存按钮点击事件
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // 验证表单
            if (!ValidateForm())
                return;

            btnSave.IsEnabled = false;

            try
            {
                if (_isEditMode && _product != null)
                {
                    // 更新商品
                    _product.FurnitureName = txtFurnitureName.Text.Trim();
                    _product.CategoryId = (int?)cmbCategory.SelectedValue;
                    _product.Model = txtModel.Text.Trim();
                    _product.PurchasePrice = decimal.Parse(txtPurchasePrice.Text.Trim());
                    _product.SalePrice = decimal.Parse(txtSalePrice.Text.Trim());
                    _product.Description = txtDescription.Text.Trim();
                    
                    var response = await _apiService.UpdateFurnitureAsync(_product.FurnitureId, _product);
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
                    // 创建新商品
                    var newProduct = new Furniture
                    {
                        FurnitureName = txtFurnitureName.Text.Trim(),
                        CategoryId = (int?)cmbCategory.SelectedValue,
                        Model = txtModel.Text.Trim(),
                        PurchasePrice = decimal.Parse(txtPurchasePrice.Text.Trim()),
                        SalePrice = decimal.Parse(txtSalePrice.Text.Trim()),
                        Description = txtDescription.Text.Trim(),
                        CreatedBy = _currentUserId,
                        CreatedAt = DateTime.Now
                    };
                    
                    var createdProduct = await _apiService.CreateFurnitureAsync(newProduct);
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
            catch (Exception ex)
            {
                ShowError("保存商品失败，请重试");
                Console.WriteLine($"保存商品失败: {ex.Message}");
            }
            finally
            {
                btnSave.IsEnabled = true;
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
            
            // 验证进货价格
            if (string.IsNullOrWhiteSpace(txtPurchasePrice.Text))
            {
                ShowError("请输入进货价格");
                return false;
            }
            
            if (!decimal.TryParse(txtPurchasePrice.Text, out decimal purchasePrice) || purchasePrice < 0)
            {
                ShowError("请输入有效的进货价格");
                return false;
            }
            
            // 验证销售价格
            if (string.IsNullOrWhiteSpace(txtSalePrice.Text))
            {
                ShowError("请输入销售价格");
                return false;
            }
            
            if (!decimal.TryParse(txtSalePrice.Text, out decimal salePrice) || salePrice < 0)
            {
                ShowError("请输入有效的销售价格");
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
            var textBox = sender as TextBox;
            string newText = textBox?.Text.Insert(textBox.SelectionStart, e.Text) ?? e.Text;
            Regex regex = new Regex(@"^\d*\.?\d{0,2}$");
            e.Handled = !regex.IsMatch(newText);
        }
    }
}
