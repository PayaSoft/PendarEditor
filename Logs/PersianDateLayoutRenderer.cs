namespace Paya.Automation.Editor.Logs
{
	using System.ComponentModel;
	using System.Globalization;
	using System.Text;

	using NLog;
	using NLog.Config;
	using NLog.LayoutRenderers;

	/// <summary>
	///     Specifies the <see cref="PersianDateLayoutRenderer" /> class.
	/// </summary>
	[LayoutRenderer("persianDate")]
	[ThreadAgnostic]
	public sealed class PersianDateLayoutRenderer : LayoutRenderer
	{
		public const string DefaultDateFormatString = "{0:0000}/{1:00}/{2:00}-{3:00}:{4:00}:{5:00}.{6:000}";

		/// <summary>
		/// Initializes a new instance of the <see cref="PersianDateLayoutRenderer"/> class.
		/// </summary>

		public PersianDateLayoutRenderer()
		{
			this.Format = DefaultDateFormatString;
		}

		#region Static Fields

		private static readonly PersianCalendar _PersianCalendar = new PersianCalendar();

		#endregion

		#region Methods

		/// <summary>
		///     Renders the specified environmental information and appends it to the specified
		///     <see cref="T:System.Text.StringBuilder" />.
		/// </summary>
		/// <param name="builder">The <see cref="T:System.Text.StringBuilder" /> to append the rendered data to.</param>
		/// <param name="logEvent">Logging event.</param>
		protected override void Append(StringBuilder builder, LogEventInfo logEvent)
		{
			if (builder == null || logEvent == null)
				return;

			var now = logEvent.TimeStamp;

			builder.AppendFormat("{0:0000}/{1:00}/{2:00}-{3:00}:{4:00}:{5:00}.{6:000}", _PersianCalendar.GetYear(now), _PersianCalendar.GetMonth(now), _PersianCalendar.GetDayOfMonth(now), _PersianCalendar.GetHour(now), _PersianCalendar.GetMinute(now), _PersianCalendar.GetSecond(now), _PersianCalendar.GetMilliseconds(now));
		}

		#endregion

		/// <summary>
		/// Gets or sets the format string of the date.
		/// </summary>
		/// <value>
		/// The format.
		/// </value>
		[DefaultParameter, DefaultValue(DefaultDateFormatString)]
		public string Format { get; set; }
	}
}