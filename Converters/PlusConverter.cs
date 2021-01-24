using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paya.Automation.Editor.Converters
{
    using System.Globalization;
    using System.Windows.Data;

    public sealed class PlusConverter : IValueConverter
    {
        public Type ItemType { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = this.ItemType ?? (value != null ? value.GetType() : typeof (int));

            dynamic d = System.Convert.ChangeType(value, type, culture);
            dynamic l = System.Convert.ChangeType(parameter, type, culture);

            return System.Convert.ChangeType(d + l, targetType, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = this.ItemType ?? (value != null ? value.GetType() : typeof(int));

            dynamic d = System.Convert.ChangeType(value, type, culture);
            dynamic l = System.Convert.ChangeType(parameter, type, culture);

            return System.Convert.ChangeType(d - l, targetType, culture);
        }
    }
}
