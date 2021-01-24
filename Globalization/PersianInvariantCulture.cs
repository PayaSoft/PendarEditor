namespace Paya.Automation.Editor.Globalization
{
	using System.Globalization;

	/// <summary>
	///     Summary description for PersianCulture
	/// </summary>
	public sealed class PersianInvariantCulture : CultureInfo
	{
		#region Constructors and Destructors

		public PersianInvariantCulture()
			: base("fa-IR")
		{
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the calendar.
		/// </summary>
		/// <value>
		/// The calendar.
		/// </value>
		public override Calendar Calendar
		{
			get
			{
				return InvariantCulture.Calendar;
			}
		}

		/// <summary>
		/// Gets or sets the date time format.
		/// </summary>
		/// <value>
		/// The date time format.
		/// </value>
		public override DateTimeFormatInfo DateTimeFormat
		{
			get
			{
				return InvariantCulture.DateTimeFormat;
			}
			set
			{
				base.DateTimeFormat = value;
			}
		}

		/// <summary>
		/// Gets or sets the number format.
		/// </summary>
		/// <value>
		/// The number format.
		/// </value>
		public override NumberFormatInfo NumberFormat
		{
			get
			{
				return InvariantCulture.NumberFormat;
			}
			set
			{
				base.NumberFormat = value;
			}
		}

		/// <summary>
		/// Gets the optional calendars.
		/// </summary>
		/// <value>
		/// The optional calendars.
		/// </value>
		public override Calendar[] OptionalCalendars
		{
			get
			{
				return InvariantCulture.OptionalCalendars;
			}
		}

		/// <summary>
		/// Gets the text information.
		/// </summary>
		/// <value>
		/// The text information.
		/// </value>
		public override TextInfo TextInfo
		{
			get
			{
				return InvariantCulture.TextInfo;
			}
		}

		#endregion
	}
}