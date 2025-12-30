using System;
using System.Globalization;
using System.Windows.Data;

namespace FurnitureManagement.Client.Converters
{
    public class StatusToChineseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "未知";

            string status = value.ToString();
            return status.ToLower() switch
            {
                "pending" => "待处理",
                "completed" => "已完成",
                "cancelled" => "已取消",
                _ => status
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}