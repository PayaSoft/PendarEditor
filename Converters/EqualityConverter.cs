using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paya.Automation.Editor.Converters
{
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public sealed class EqualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result;

            if (Equals(value, parameter))
                result = true;
            else
            {
                if (value == null || parameter == null)
                    result = false;
                else
                {
                    var valueType = value.GetType();
                    valueType = Nullable.GetUnderlyingType(valueType) ?? valueType;

                    var paramValue = System.Convert.ChangeType(parameter, valueType, culture);

                    result = Equals(value, paramValue);
                }
            }

            targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (targetType == typeof (bool))
                return result;

            if (targetType == typeof (Visibility))
                return result ? Visibility.Visible : Visibility.Collapsed;

            return System.Convert.ChangeType(result, targetType, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        
    }
}
