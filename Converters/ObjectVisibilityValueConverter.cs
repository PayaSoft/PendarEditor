namespace Paya.Automation.Editor.Converters
{
	using System;
	using System.Diagnostics;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Data;

	/// <summary>
	/// A type converter for converting boolean or string values to visibility.
	/// </summary>
	public class ObjectVisibilityValueConverter : IValueConverter
	{
		#region Public Methods

		/// <summary>
		/// Converts the bool to visibility.
		/// </summary>
		/// <param name="isVisible">
		/// if set to <c>true</c> returns <see cref="Visibility.Visible"/>; otherwise <see cref="Visibility.Collapsed"/>.
		/// </param>
		/// <returns>
		/// if set to <c>true</c> returns <see cref="Visibility.Visible"/>; otherwise <see cref="Visibility.Collapsed"/>.
		/// </returns>
		public static Visibility ConvertBoolToVisibility(bool isVisible)
		{
			return isVisible ? Visibility.Visible : Visibility.Collapsed;
		}

		#endregion

		#region Implemented Interfaces

		#region IValueConverter

		/// <summary>
		/// Modifies the source data before passing it to the target for display in the UI.
		/// </summary>
		/// <param name="value">
		/// The source data being passed to the target. Acceptable types are boolean or string.
		/// </param>
		/// <param name="targetType">
		/// The <see cref="T:System.Type"/> of data expected by the target dependency property.
		/// </param>
		/// <param name="parameter">
		/// An optional parameter to be used in the converter logic.
		/// </param>
		/// <param name="culture">
		/// The culture of the conversion.
		/// </param>
		/// <returns>
		/// A <see cref="Visibility"/> value to be passed to the target dependency property.
		/// </returns>
		/// <remarks>
		/// If the <paramref name="value"/>'s type is string, if the passing value wasn't empty, null or white space, returns 
		/// <see cref="Visibility.Visible"/>; otherwise <see cref="Visibility.Collapsed"/>
		/// </remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(Visibility))
				return null;

			var parameterString = System.Convert.ToString(parameter, culture);
		    var notValue = parameterString.Equals("NotValue", StringComparison.OrdinalIgnoreCase) || parameterString.Contains("!");
		    var compareWithZero = parameterString.Contains("0");

		    var strValue = value as string;
		    if (strValue != null)
			{
			    return Equals(strValue, "0") ? ConvertBoolToVisibility(false) : ConvertBoolToVisibility(notValue ? string.IsNullOrWhiteSpace(strValue) : !string.IsNullOrWhiteSpace(strValue));
			}

		    if (value is bool)
				return ConvertBoolToVisibility(notValue ? !((bool)value) : (bool)value);

		    if (value is Visibility)
		    {
                var v = (Visibility)value;
                return notValue ? (v == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible) : v;
		    }

		    if (value is double)
				return ConvertBoolToVisibility(compareWithZero ? (notValue ? Math.Abs((double)value) < double.Epsilon : Math.Abs((double)value) > double.Epsilon) : (notValue ? (double)value <= 0 : (double)value > 0));

			if (value is int)
				return ConvertBoolToVisibility(compareWithZero ? (notValue ? (int)value == 0 : (int)value != 0) : notValue ? (int)value <= 0 : (int)value > 0);

			if (value is decimal)
				return ConvertBoolToVisibility(compareWithZero ? (notValue ? (decimal)value == 0 : (decimal)value != 0) : notValue ? (decimal)value <= 0 : (decimal)value > 0);

			if (value is long)
				return ConvertBoolToVisibility(compareWithZero ? (notValue ? (long)value == 0 : (long)value != 0) : notValue ? (long)value <= 0 : (long)value > 0);

			if (value is float)
				return ConvertBoolToVisibility(compareWithZero ? (notValue ? Math.Abs((float)value) < float.Epsilon : Math.Abs((float)value) > float.Epsilon) : notValue ? (float)value <= 0 : (float)value > 0);

		    if (value!=null && value.GetType().IsEnum && !string.IsNullOrEmpty(parameterString))
			{
				try
				{
					parameterString = parameterString.Replace("!", string.Empty);
					var enumValue = Enum.Parse(value.GetType(), parameterString, true);
					if (enumValue != null)
					{
						bool areEqual = Equals(enumValue, value);
						return ConvertBoolToVisibility(notValue ? !areEqual : areEqual);
					}
				}
				catch (Exception exp)
				{
					Debug.WriteLine(exp);
				}
			}

		    if (value == null || value.GetType().IsClass)
		    {
                bool b = value != null;
                if (notValue)
                    b = !b;
		        return ConvertBoolToVisibility(b);
		    }

		    if (value.GetType().IsValueType)
		    {
		        bool b = !Equals(value, Activator.CreateInstance(value.GetType()));
                if (notValue)
                    b = !b;
                return ConvertBoolToVisibility(b);
            }

		    return ConvertBoolToVisibility(!notValue);
		}

		/// <summary>
		/// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
		/// </summary>
		/// <param name="value">
		/// The target data being passed to the source.
		/// </param>
		/// <param name="targetType">
		/// The <see cref="T:System.Type"/> of data expected by the source object.
		/// </param>
		/// <param name="parameter">
		/// An optional parameter to be used in the converter logic.
		/// </param>
		/// <param name="culture">
		/// The culture of the conversion.
		/// </param>
		/// <returns>
		/// A boolean or string value to be passed to the source object.
		/// </returns>
		/// <remarks>
		/// When <paramref name="targetType"/> is boolean, if the value is <see cref="Visibility.Visible"/> the return value would be <c>true</c>.
		/// When <paramref name="targetType"/> is string, if the value is <see cref="Visibility.Visible"/> the return value would be 
		/// the string representation of the <c>Visibility.Visible</c>; but if the value is <see cref="Visibility.Collapsed"/> null will be returned.
		/// </remarks>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
            if (!(value is Visibility))
                return null;

			var visibility = (Visibility)value;

			if (targetType == typeof(bool))
				return visibility == Visibility.Visible;

		    if (targetType == typeof (Visibility))
		        return visibility;

			if (targetType == typeof(string))
				return visibility == Visibility.Visible ? visibility.ToString() : null;

			if (targetType == typeof(int))
				return visibility == Visibility.Visible ? 1 : 0;

			if (targetType == typeof(long))
				return visibility == Visibility.Visible ? 1L : 0L;

			if (targetType == typeof(double))
				return visibility == Visibility.Visible ? 1d : 0d;

			if (targetType == typeof(decimal))
				return visibility == Visibility.Visible ? 1M : 0m;

		    return System.Convert.ChangeType(value, targetType, culture);
		}

		#endregion

		#endregion
	}
}