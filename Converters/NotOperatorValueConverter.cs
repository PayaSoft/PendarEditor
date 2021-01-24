namespace Paya.Automation.Editor.Converters
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows.Data;
	using Visibility = System.Windows.Visibility;

    /// <summary>
    /// Two way <see cref="IValueConverter"/> that lets you bind the inverse of a Boolean property
    /// to a dependency property
    /// </summary>
    public class NotOperatorValueConverter : IValueConverter
    {
        #region Implemented Interfaces

        #region IValueConverter

        /// <summary>
        /// Converts the given <paramref name="value"/> to be its inverse
        /// </summary>
        /// <param name="value">
        /// The <c>bool</c> value to convert.
        /// </param>
        /// <param name="targetType">
        /// The type to convert to (ignored).
        /// </param>
        /// <param name="parameter">
        /// Optional parameter (ignored).
        /// </param>
        /// <param name="culture">
        /// The culture of the conversion (ignored).
        /// </param>
        /// <returns>
        /// The inverse of the input <paramref name="value"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertNotValueToObject(value, targetType, culture);
        }

        /// <summary>
        /// The inverse of the <see cref="Convert"/>.
        /// </summary>
        /// <param name="value">
        /// The value to convert back.
        /// </param>
        /// <param name="targetType">
        /// The type to convert to (ignored).
        /// </param>
        /// <param name="parameter">
        /// Optional parameter (ignored).
        /// </param>
        /// <param name="culture">
        /// The culture of the conversion (ignored).
        /// </param>
        /// <returns>
        /// The inverse of the input <paramref name="value"/>.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertNotValueToObject(value, targetType, culture);
        }

        #endregion

        #endregion

        #region Methods

		/// <summary>Converts the not value to object.</summary>
		/// <param name="value">The value.</param>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="formatProvider">The format provider.</param>
		/// <returns></returns>
        private static object ConvertNotValueToObject(object value, Type targetType, IFormatProvider formatProvider)
        {
			try
			{
				if (value == null)
					value = Activator.CreateInstance(targetType); // the default value
			}
			catch (Exception exp)
			{
				Debug.WriteLine(exp);
			}

			if (targetType == typeof(bool) || targetType == typeof(bool?))
			{
				if (value is bool)
					return !(bool)value;

				return !System.Convert.ToBoolean(value, formatProvider);
			}

			if (value is bool && targetType == typeof(Visibility))
				return ObjectVisibilityValueConverter.ConvertBoolToVisibility(!(bool)value);

			if (value is Visibility && targetType == typeof(Visibility))
			{
				switch ((Visibility)value)
				{
					case Visibility.Collapsed:
					case Visibility.Hidden:
						return Visibility.Visible;

					case Visibility.Visible:
						return Visibility.Collapsed;
				}
			}

			var convertible = value as IConvertible;
			if (convertible != null)
				try
				{
					dynamic negative = convertible is string ? -System.Convert.ToDecimal(convertible, formatProvider) : -(dynamic)convertible;
					return System.Convert.ChangeType(negative, targetType, formatProvider);
				}
				catch (Exception exp)
				{
					Debug.WriteLine(exp);
				}

			return null;
        }

        #endregion
    }
}