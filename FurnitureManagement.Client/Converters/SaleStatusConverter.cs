using System;
using System.Globalization;
using System.Windows.Data;

namespace FurnitureManagement.Client.Converters
{
    /// <summary>
    /// 销售订单状态转换器，用于将英文状态转换为中文显示
    /// </summary>
    public class SaleStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                // 建立中英文状态映射关系
                switch (status.ToLower())
                {
                    case "pending":
                        return "待处理";
                    case "completed":
                        return "已完成";
                    case "cancelled":
                        return "已取消";
                    default:
                        return status;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // ConvertBack 不需要实现，因为我们只在显示时转换为中文
            throw new NotImplementedException();
        }
    }
}