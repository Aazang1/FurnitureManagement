using System;
using System.Globalization;
using System.Windows.Data;

namespace FurnitureManagement.Client.Converters
{
    public class FlowTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string flowType)
            {
                switch (flowType.ToLower())
                {
                    case "income":
                        return "收入";
                    case "expense":
                        return "支出";
                    default:
                        return flowType;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}