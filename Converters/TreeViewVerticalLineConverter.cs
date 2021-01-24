using System;

namespace Paya.Automation.Editor.Converters
{
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Data;

    public sealed class TreeViewVerticalLineConverter : IValueConverter
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
            if (ic == null)
                return 0;
            //int index = ic.ItemContainerGenerator.IndexFromContainer(item);

            if (!"top".Equals(System.Convert.ToString(parameter, culture), StringComparison.OrdinalIgnoreCase))
                return item.HasItems ? 1 : 0;

            if (ic is TreeView)
                return 0;

            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}