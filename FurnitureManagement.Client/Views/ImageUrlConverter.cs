using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace FurnitureManagement.Client.Views
{
    /// <summary>
    /// 图片URL转换器，用于将相对路径转换为完整的图片URL
    /// </summary>
    public class ImageUrlConverter : IValueConverter
    {
        /// <summary>
        /// 将相对路径转换为BitmapImage
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string imageUrl = value as string;
            
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                // 返回空图片
                return null;
            }
            
            try
            {
                // 构建完整的图片URL
                string fullUrl = $"http://localhost:5192/images/{imageUrl}";
                return new BitmapImage(new Uri(fullUrl));
            }
            catch (Exception)
            {
                // 如果URL无效，返回空图片
                return null;
            }
        }

        /// <summary>
        /// 反向转换（不使用）
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}