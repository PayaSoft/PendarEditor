using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paya.Automation.Editor.Converters
{
    using System.Globalization;
    using System.Windows.Data;

    public sealed class ObjectToBooleanConverter : IValueConverter
    {
        #region Public Methods and Operators

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof (bool) || targetType == typeof (bool?))
            {
                bool isNot = Equals(parameter, "!");
                bool r = value != null;
                return isNot ? !r : r;
            }

            return System.Convert.ChangeType(value, targetType, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ChangeType(value, targetType, culture);
        }

        #endregion
    }
}
