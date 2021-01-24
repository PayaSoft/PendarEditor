using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paya.Automation.Editor.Converters
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class EnumBooleanConverter : IValueConverter
    {
        #region Public Methods and Operators

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            var valueType = value.GetType();

            var ParameterString = System.Convert.ToString(parameter, culture);

            if (valueType.IsEnum && (targetType == typeof(bool) || targetType == typeof(bool?)) )
            {
                if (ParameterString.Length == 0)
                    return DependencyProperty.UnsetValue;

                if (Enum.IsDefined(valueType, value) == false)
                    return DependencyProperty.UnsetValue;

                object paramvalue = Enum.Parse(valueType, ParameterString.Trim());
                return paramvalue.Equals(value);
            }

            if ((value is bool) && targetType.IsEnum && parameter!=null)
            {
                bool b = (bool)value;

                var paramStr = System.Convert.ToString(parameter, culture);

                if (paramStr.Contains("!"))
                {
                    b = !b;
                    paramStr = paramStr.Replace("!", "");
                }

                var values = paramStr.Split(',', '+', ';', '&', '|', ':', '/', '\\');

                try
                {
                    return values.Length < 2 ? DependencyProperty.UnsetValue : Enum.Parse(targetType, values[b ? 0 : 1].Trim(), true);
                }
                catch (Exception exp)
                {
                    Debug.WriteLine(exp);
                    return DependencyProperty.UnsetValue;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            var ParameterString = System.Convert.ToString(parameter, culture);

            if (value is bool && targetType.IsEnum)
            {
                var valueAsBool = (bool) value;

                if (ParameterString.Length == 0 || !valueAsBool)
                {
                    try
                    {
                        return Enum.Parse(targetType, "0");
                    }
                    catch (Exception exp)
                    {
                        Debug.WriteLine(exp);
                        return DependencyProperty.UnsetValue;
                    }
                }

                return Enum.Parse(targetType, ParameterString.Trim());
            }

            var valueType = value.GetType();
            if (valueType.IsEnum && (targetType == typeof (bool) || targetType == typeof (bool?)))
            {
                bool isNot = false;
                if (ParameterString.Contains("!"))
                {
                    isNot = true;
                    ParameterString = ParameterString.Replace("!", "");
                }

                var values = ParameterString.Split(',', '+', ';', '&', '|', ':', '/', '\\');

                if (values.Length < 2)
                    return DependencyProperty.UnsetValue;

                try
                {
                    bool? b = Equals(Enum.Parse(targetType, values[0].Trim(), true), value) ? true : Equals(Enum.Parse(targetType, values[1].Trim(), true), value) ? (bool?) false : null;
                    return (isNot ? !b : b) ?? DependencyProperty.UnsetValue;
                }
                catch (Exception exp)
                {
                    Debug.WriteLine(exp);
                    return DependencyProperty.UnsetValue;
                }
            }

            return DependencyProperty.UnsetValue;
        }

        #endregion
    }
}
