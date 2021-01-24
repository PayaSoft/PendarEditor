using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paya.Automation.Editor.Converters
{
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Data;

    public sealed class TreeViewHorizontalLineConverter : IValueConverter
    {
        #region Public Methods and Operators

        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as TreeViewItem;
            if (item == null)
                return 0;
            var ic = ItemsControl.ItemsControlFromItemContainer(item);
            if (ic == null || ic.ItemContainerGenerator == null)
                return 0;
            var index = ic.ItemContainerGenerator.IndexFromContainer(item);

            if ("left".Equals(System.Convert.ToString(parameter, culture), StringComparison.OrdinalIgnoreCase))
            {
                return index == 0 ? 0 : 1;
            }

            return index == ic.Items.Count - 1 ? 0 : 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}
