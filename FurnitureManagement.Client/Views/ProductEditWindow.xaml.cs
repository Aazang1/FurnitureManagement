using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FurnitureManagement.Client.Models;
using FurnitureManagement.Client.Servcie;
using Microsoft.Win32;

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
                txtImageUrl.Text = _product.ImageUrl ?? string.Empty;

                // 如果有图片URL，则加载图片
                if (!string.IsNullOrWhiteSpace(_product.ImageUrl))
                {
                    txtImagePath.Text = _product.ImageUrl;
                    try
                    {
                        // 构建完整的图片URL
                        string imageUrl = $"http://localhost:5192/images/{_product.ImageUrl}";
                        var bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri(imageUrl);
                        bitmapImage.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        imgPreview.Source = bitmapImage;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"加载商品图片失败: {ex.Message}");
                    }
                }
            }
        }

        // 图片上传按钮点击事件
        private async void btnUploadImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 创建文件选择对话框
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "选择商品图片";
                openFileDialog.Filter = "图片文件 (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|所有文件 (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;
                    
                    // 检查文件大小（限制为5MB）
                    var fileInfo = new System.IO.FileInfo(filePath);
                    if (fileInfo.Length > 5 * 1024 * 1024) // 5MB
                    {
                        MessageBox.Show("图片文件大小不能超过5MB", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // 显示图片预览
                    var bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(filePath);
                    bitmapImage.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    imgPreview.Source = bitmapImage;
                    
                    // 显示上传状态
                    txtImagePath.Text = "正在上传...";
                    btnUploadImage.IsEnabled = false;

                    // 上传图片到服务器
                    var uploadResponse = await _apiService.UploadImageAsync(filePath);
                    
                    if (uploadResponse != null && uploadResponse.Success)
                    {
                        // 更新UI，保存服务器返回的相对路径
                        string imageFileName = uploadResponse.FileName;
                        txtImagePath.Text = imageFileName;
                        txtImageUrl.Text = imageFileName; // 存储图片文件名（相对路径）
                        MessageBox.Show("图片上传成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("图片上传失败，请重试", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        // 重置UI
                        imgPreview.Source = null;
                        txtImagePath.Text = string.Empty;
                        txtImageUrl.Text = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"上传图片失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"上传图片失败: {ex.Message}");
            }
            finally
            {
                btnUploadImage.IsEnabled = true;
            }
        }

        // 清空图片按钮点击事件
        private void btnClearImage_Click(object sender, RoutedEventArgs e)
        {
            ClearImage();
        }

        // 清空图片
        private void ClearImage()
        {
            imgPreview.Source = null;
            txtImagePath.Text = string.Empty;
            txtImageUrl.Text = string.Empty;

            // 如果是编辑模式，清除原有的图片URL
            if (_isEditMode && _product != null)
            {
                _product.ImageUrl = null;
            }
        }

        // 拖拽进入事件
        private void Image_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        // 拖拽悬停事件
        private void Image_DragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        // 拖拽放置事件
        private async void Image_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length > 0)
                {
                    string filePath = files[0];

                    // 检查文件类型
                    string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    string fileExtension = System.IO.Path.GetExtension(filePath).ToLower();

                    if (imageExtensions.Contains(fileExtension))
                    {
                        // 检查文件大小（限制为5MB）
                        var fileInfo = new System.IO.FileInfo(filePath);
                        if (fileInfo.Length > 5 * 1024 * 1024) // 5MB
                        {
                            MessageBox.Show("图片文件大小不能超过5MB", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        try
                        {
                            // 显示上传状态
                            txtImagePath.Text = "正在上传...";
                            btnUploadImage.IsEnabled = false;

                            // 上传图片到服务器
                            var uploadResponse = await _apiService.UploadImageAsync(filePath);
                            
                            if (uploadResponse != null && uploadResponse.Success)
                            {
                                // 更新UI，保存服务器返回的相对路径
                                string imageFileName = uploadResponse.FileName;
                                txtImagePath.Text = imageFileName;
                                txtImageUrl.Text = imageFileName; // 存储图片文件名（相对路径）
                                
                                // 显示图片预览
                                string imageUrl = $"http://localhost:5192/images/{imageFileName}";
                                var bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
                                bitmapImage.BeginInit();
                                bitmapImage.UriSource = new Uri(imageUrl);
                                bitmapImage.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                                bitmapImage.EndInit();
                                imgPreview.Source = bitmapImage;
                                
                                MessageBox.Show("图片上传成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("图片上传失败，请重试", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                                // 重置UI
                                imgPreview.Source = null;
                                txtImagePath.Text = string.Empty;
                                txtImageUrl.Text = string.Empty;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"上传图片失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                            Console.WriteLine($"上传图片失败: {ex.Message}");
                            // 重置UI
                            imgPreview.Source = null;
                            txtImagePath.Text = string.Empty;
                            txtImageUrl.Text = string.Empty;
                        }
                        finally
                        {
                            btnUploadImage.IsEnabled = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("请选择有效的图片文件（JPG, PNG, GIF, BMP）", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
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

                    // 更新图片URL（使用txtImageUrl中存储的服务器返回的相对路径）
                    _product.ImageUrl = !string.IsNullOrWhiteSpace(txtImageUrl.Text) ? txtImageUrl.Text : null;

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
                        CreatedAt = DateTime.Now,
                        ImageUrl = !string.IsNullOrWhiteSpace(txtImageUrl.Text) ? txtImageUrl.Text : null
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