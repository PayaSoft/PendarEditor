using System;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;
using Paya.Properties;

namespace Paya.Automation
{
    using System;

    /// <summary>
    ///     Specifies the <see cref="DateTimeUtility" /> class.
    /// </summary>
    [PublicAPI]
    public static class DateTimeUtility
    {
        #region Static Fields

        private static readonly PersianCalendar _PersianCalendar = new PersianCalendar();

        private static readonly DateTimeFormatInfo _PersianDateTimeFormatInfo;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes the <see cref="DateTimeUtility" /> class.
        /// </summary>
        static DateTimeUtility()
        {
            var info = (DateTimeFormatInfo)CultureInfo.GetCultureInfo("fa-IR").DateTimeFormat.Clone();
            info.AbbreviatedDayNames = new[] { "ی", "د", "س", "چ", "پ", "ج", "ش" };
            info.DayNames = new[] { "یکشنبه", "دوشنبه", "سه‌شنبه", "چهارشنبه", "پنجشنبه", "جمعه", "شنبه" };
            info.AbbreviatedMonthNames = new[] { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند", "" };
            info.MonthNames = new[] { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند", "" };
            info.AMDesignator = "ق.ظ";
            info.PMDesignator = "ب.ظ";
            info.ShortDatePattern = "yyyy/MM/dd";
            info.FirstDayOfWeek = DayOfWeek.Saturday;
            _PersianDateTimeFormatInfo = DateTimeFormatInfo.ReadOnly(info);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the persian calendar.
        /// </summary>
        /// <value>
        ///     The persian calendar.
        /// </value>
        public static PersianCalendar PersianCalendar
        {
            get { return _PersianCalendar; }
        }

        /// <summary>
        ///     Gets the persian date time format information.
        /// </summary>
        /// <value>
        ///     The persian date time format information.
        /// </value>
        public static DateTimeFormatInfo PersianDateTimeFormatInfo
        {
            get { return _PersianDateTimeFormatInfo; }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Converts the date time to string.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="format">The format.</param>
        /// <param name="calendar">The calendar.</param>
        /// <param name="dateTimeFormat">The date time format.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">calendar</exception>
        /// <remarks>
        ///     <para> Date Time Formats </para>
        ///     <para xml:space="preserve">
        /// d - Numeric day of the month without a leading zero.
        /// dd - Numeric day of the month with a leading zero.
        /// ddd - Abbreviated name of the day of the week.
        /// dddd - Full name of the day of the week.
        /// f,ff,fff,ffff,fffff,ffffff,fffffff - Fraction of a second. The more Fs the higher the precision.
        /// h - 12 Hour clock, no leading zero.
        /// hh - 12 Hour clock with leading zero.
        /// H - 24 Hour clock, no leading zero.
        /// HH - 24 Hour clock with leading zero.
        /// m - Minutes with no leading zero.
        /// mm - Minutes with leading zero.
        /// M - Numeric month with no leading zero.
        /// MM - Numeric month with a leading zero.
        /// MMM - Abbreviated name of month.
        /// MMMM - Full month name.
        /// s - Seconds with no leading zero.
        /// ss - Seconds with leading zero.
        /// t - AM/PM but only the first letter.
        /// tt - AM/PM ( a.m. / p.m.)
        /// y - Year with out century and leading zero.
        /// yy - Year with out century, with leading zero.
        /// yyyy - Year with century.
        /// zz - Time zone off set with +/-.
        /// </para>
        /// </remarks>
        [NotNull]
        public static string ConvertDateTimeToString(DateTime date, [CanBeNull]string format, [CanBeNull] Calendar calendar, [NotNull] DateTimeFormatInfo dateTimeFormat)
        {
            if (dateTimeFormat == null)
                throw new ArgumentNullException("dateTimeFormat");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            if (calendar == null)
                calendar = dateTimeFormat.Calendar;

            if (string.IsNullOrEmpty(format))
                format = dateTimeFormat.ShortDatePattern;

            int n = format.Length;

            var result = new StringBuilder(2 * n + 1);

            for (int i = 0; i < format.Length; i++)
            {
                if (format[i] == '%' && i + 1 < n)
                {
                    result.Append(format[++i]);
                    continue;
                }

                string pattern;
                var lastIndex = FindLastIndexOfRepeatedChar(format, i, out pattern);
                i = lastIndex;

                switch (pattern)
                {
                    case "d":
                        result.Append(calendar.GetDayOfMonth(date));
                        break;
                    case "dd":
                        result.Append(calendar.GetDayOfMonth(date).ToString("00"));
                        break;
                    case "ddd":
                        result.Append(dateTimeFormat.GetAbbreviatedDayName(calendar.GetDayOfWeek(date)));
                        break;
                    case "dddd":
                        result.Append(dateTimeFormat.GetDayName(calendar.GetDayOfWeek(date)));
                        break;

                    case "f":
                    case "ff":
                    case "fff":
                    case "ffff":
                    case "fffff":
                    case "ffffff":
                    case "fffffff":
                    case "F":
                    case "FF":
                    case "FFF":
                    case "FFFF":
                    case "FFFFF":
                    case "FFFFFF":
                    case "FFFFFFF":
                        {
                            var f = (int)Math.Round((date - new DateTime(date.Ticks - date.Ticks % TimeSpan.TicksPerSecond, date.Kind)).TotalSeconds * Math.Pow(10, pattern.Length));
                            result.Append(f.ToString("D" + pattern.Length));
                        }
                        break;

                    case "h":
                        result.Append(calendar.GetHour(date) % 12);
                        break;
                    case "hh":
                        result.Append((calendar.GetHour(date) % 12).ToString("00"));
                        break;

                    case "H":
                        result.Append(calendar.GetHour(date));
                        break;
                    case "HH":
                        result.Append(calendar.GetHour(date).ToString("00"));
                        break;

                    case "m":
                        result.Append(calendar.GetMinute(date));
                        break;
                    case "mm":
                        result.Append(calendar.GetMinute(date).ToString("00"));
                        break;

                    case "M":
                        result.Append(calendar.GetMonth(date));
                        break;
                    case "MM":
                        result.Append(calendar.GetMonth(date).ToString("00"));
                        break;

                    case "MMM":
                        result.Append(dateTimeFormat.GetAbbreviatedMonthName(calendar.GetMonth(date)));
                        break;
                    case "MMMM":
                        result.Append(dateTimeFormat.GetMonthName(calendar.GetMonth(date)));
                        break;

                    case "s":
                        result.Append(calendar.GetSecond(date));
                        break;
                    case "ss":
                        result.Append((calendar.GetSecond(date)).ToString("00"));
                        break;

                    case "t":
                    case "tt":
                        {
                            int hour = calendar.GetHour(date);
                            result.Append(hour < 12 ? dateTimeFormat.AMDesignator : dateTimeFormat.PMDesignator);
                        }
                        break;

                    case "y":
                        result.Append(calendar.GetYear(date) % 100);
                        break;
                    case "yy":
                        result.Append((calendar.GetYear(date) % 100).ToString("00"));
                        break;

                    case "yyyy":
                    case "yyy":
                        result.Append(calendar.GetYear(date));
                        break;

                    case "zz":
                    case "zzz":
                    case "zzzz":
                    case "zzzzz":
                    case "zzzzzz":
                        result.Append(date.ToString(pattern));
                        break;


                    default:
                        result.Append(pattern);
                        break;
                }
            }

            return result.ToString();
        }

        /// <summary>
        ///     Gets the day of week string.
        /// </summary>
        /// <param name="day"> The day. </param>
        /// <returns> </returns>
        [NotNull]
        public static string GetDayOfWeekString(DayOfWeek day)
        {
            return DateResources.ResourceManager.GetString(string.Format("DayOfWeek_{0}", day)) ?? string.Empty;
        }

        /// <summary>
        ///     Gets the name of the month.
        /// </summary>
        /// <param name="month">The month number(1..12).</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The <paramref name="month" /> is not between 1 and 12</exception>
        [NotNull]
        public static string GetMonthName(int month)
        {
            if (month < 1 || month > 12)
            {
                throw new ArgumentOutOfRangeException("month", month, DateResources.InvalidMonthValue);
            }
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            return CalendarMonths.ResourceManager.GetString("Month" + month) ?? string.Empty;
        }

        [Pure, NotNull]
        public static string ToCultureString(this DateTime date, [CanBeNull] string format = null)
        {
            var culture = CultureInfo.CurrentCulture;
            return date.ToCultureString(culture, format);
        }

        [Pure, NotNull]
        public static string ToCultureString(this DateTime date, [NotNull] CultureInfo culture, [CanBeNull] string format = null)
        {
            if (culture == null)
                throw new ArgumentNullException("culture");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            return culture.Name.StartsWith("fa", StringComparison.OrdinalIgnoreCase)
                ? ConvertDateTimeToString(date, format, PersianCalendar, PersianDateTimeFormatInfo)
                : date.ToString(format, culture);
        }

        [ContractAnnotation("date:null=>null;date:notnull=>notnull"), Pure]
        public static string ToCultureString(this DateTime? date, [CanBeNull] string format = null)
        {
            return date == null ? null : date.Value.ToCultureString(format);
        }

        [Pure]
        [ContractAnnotation("null=>null;notnull=>notnull")]
        public static string ToJson(this DateTime? date)
        {
            return date == null ? null : date.Value.ToJson();
        }

        [NotNull]
        public static string ToJson(this DateTime time)
        {
            // "2013-06-13T10:07:53.477Z"

            return time.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", CultureInfo.InvariantCulture);
        }

        [NotNull]
        public static string ToLongPersianDateString(this DateTime date)
        {
            int year = PersianCalendar.GetYear(date);
            int month = PersianCalendar.GetMonth(date);
            int day = PersianCalendar.GetDayOfMonth(date);
            DayOfWeek dayOfWeek = PersianCalendar.GetDayOfWeek(date);

            return string.Format("{0}، {1} {2} {3}", GetDayOfWeekString(dayOfWeek), day, GetMonthName(month), year);
        }

        #endregion

        #region Methods

        private static int FindLastIndexOfRepeatedChar([NotNull] string format, int currentIndex, [NotNull] out string pattern)
        {
            var outChars = new StringBuilder(8);
            char c = format[currentIndex];
            outChars.Append(c);
            int i;
            int n = format.Length;
            for (i = currentIndex + 1; i < n; i++)
            {
                char sc = format[i];
                if (sc != c)
                {
                    i--;
                    break;
                }

                outChars.Append(sc);
            }

            pattern = outChars.ToString();
            return i < n ? i : n - 1;
        }

        #endregion
    }
}
