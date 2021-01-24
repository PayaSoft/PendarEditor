namespace Paya.Automation.Editor.Converters
{
	using System;
	using System.Collections;
	using System.Globalization;
	using System.Text;
	using System.Windows;
	using System.Windows.Data;

	/// <summary>
	///     Specifies the <see cref="ListToStringConverter" /> class.
	/// </summary>
	public sealed class ListToStringConverter : DependencyObject, IValueConverter
	{
		#region Constants

		/// <summary>
		///     The <see cref="Seperator" /> dependency property's name.
		/// </summary>
		public const string SeperatorPropertyName = "Seperator";

		#endregion

		#region Static Fields

		/// <summary>
		///     Identifies the <see cref="Seperator" /> dependency property.
		/// </summary>
		public static readonly DependencyProperty SeperatorProperty = DependencyProperty.Register(SeperatorPropertyName, typeof(string), typeof(ListToStringConverter), new UIPropertyMetadata(Environment.NewLine));

		#endregion

		#region Public Properties

		/// <summary>
		///     Gets or sets the value of the <see cref="Seperator" />
		///     property. This is a dependency property.
		/// </summary>
		public string Seperator
		{
			get
			{
				return (string)this.GetValue(SeperatorProperty);
			}
			set
			{
				this.SetValue(SeperatorProperty, value);
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///     Converts a value.
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		///     A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return null;
			}
			if (value is string)
			{
				return value;
			}

			var sb = new StringBuilder();
			var ie = value as IEnumerable;
			if (ie != null)
			{
				foreach (object o in ie)
				{
					sb.Append(o).Append(this.Seperator);
				}
			}

			return sb.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}

		#endregion
	}
}