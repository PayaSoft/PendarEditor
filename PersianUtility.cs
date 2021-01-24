namespace Paya
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    using Paya.Properties;

    /// <summary>
    ///     Specifies the <see cref="PersianUtility" /> static class
    /// </summary>
    [JetBrains.Annotations.PublicAPI]
    [System.Diagnostics.Contracts.Pure]
    [SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
    public static partial class PersianUtility
    {
        #region Static Fields

        private static readonly Calendar _PersianCalendar = new PersianCalendar(), _GregorianCalendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish), _HijriCalendar = new HijriCalendar();

        private static readonly Regex _DateTimeRegx = new Regex(@"^\D*(((?<year>[0-9]+)\D+(?<month>[0-9]+)\D+(?<day>[0-9]+))|((?<year>((1[34][0-9]{2})|[0-9]{2})))(?<month>((10|11|12)|(0?[0-9])))(?<day>([123][0-9])|(0?[0-9]{1})))((\s|T)+(?<hour>[012]?[0-9])\:(?<minutes>[0-5]?[0-9])(\:(?<seconds>[0-5]?[0-9])(\.(?<ms>[0-9]{1,3}))?)?)?.*?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline | RegexOptions.CultureInvariant);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets the name of the month.
        /// </summary>
        /// <param name="month">The month number(1..12).</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The <paramref name="month"/> is not between 1 and 12</exception>
        [JetBrains.Annotations.NotNull]
        public static string GetMonthName(int month)
        {
            if (month < 1 || month > 12)
            {
                throw new ArgumentOutOfRangeException("month", month, DateResources.InvalidMonthValue);
            }

            return CalendarMonths.ResourceManager.GetString("Month" + month) ?? string.Empty;
        }

        public static DateTime? ParseCultureDate([JetBrains.Annotations.CanBeNull] string date)
        {
            var calendar = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.StartsWith("fa", StringComparison.OrdinalIgnoreCase) ? _PersianCalendar : CultureInfo.CurrentCulture.Calendar;
            return ParseDate(date, false, calendar, calendar == _PersianCalendar ? 13 : 20);
        }

        public static DateTime? ParseHijriDate([JetBrains.Annotations.CanBeNull]string date)
        {
            return ParseDate(date, true, _HijriCalendar, 13);
        }

        public static DateTime? ParseGregorianDate([JetBrains.Annotations.CanBeNull] string date)
        {
            //return ParseDate(date, true, _GregorianCalendar, 19);
            DateTime result;
            if (string.IsNullOrEmpty(date) || !DateTime.TryParse(date, out result))
                return null;
            return result;
        }

        /// <summary>
        ///     Parses the given Persian date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static DateTime? ParsePersianDate([JetBrains.Annotations.CanBeNull]string date)
        {
            return ParseDate(date, true, _PersianCalendar, 13);
        }

        public static DateTime? ParsePersianDateTime([JetBrains.Annotations.CanBeNull]string dateTimeString)
        {
            return ParseDate(dateTimeString, false, _PersianCalendar, 13);
        }

        public static DateTime? ParseGregorianDateTime([JetBrains.Annotations.CanBeNull]string dateTimeString)
        {
            return ParseDate(dateTimeString, false, _GregorianCalendar, 19);
        }

        public static DateTime? ParseHijriDateTime([JetBrains.Annotations.CanBeNull]string dateTimeString)
        {
            return ParseDate(dateTimeString, false, _HijriCalendar, 13);
        }

        [JetBrains.Annotations.ContractAnnotation("dateTimeString:null=>null;"), JetBrains.Annotations.CanBeNull]
        public static DateTime? ParseDate([JetBrains.Annotations.CanBeNull]string dateTimeString, bool ignoreTime, [JetBrains.Annotations.NotNull] Calendar calendar, int century)
        {
            if (calendar == null)
                throw new ArgumentNullException("calendar");

            if (dateTimeString == null)
                return null;

            Match m = _DateTimeRegx.Match(dateTimeString);
            if (!m.Success)
            {
                return null;
            }

            int year = int.Parse(m.Groups["year"].Value);
            int month = int.Parse(m.Groups["month"].Value);
            int day = int.Parse(m.Groups["day"].Value);

            if (year < 55)
            {
                year += (century + 1) * 100;
            }
            else if (year < 100)
            {
                year += century * 100;
            }

            if (month < 1 || month > 12)
            {
                return null;
            }

            if (day < 1 || day > calendar.GetDaysInMonth(year, month))
            {
                return null;
            }

            if (ignoreTime)
                return new DateTime(year, month, day, calendar);

            var hg = m.Groups["hour"];
            var mg = m.Groups["minutes"];
            var sg = m.Groups["seconds"];
            var msg = m.Groups["ms"];

            int hour = hg.Success ? int.Parse(hg.Value) : 0;
            int minutes = mg.Success ? int.Parse(mg.Value) : 0;
            int seconds = sg.Success ? int.Parse(sg.Value) : 0;
            int ms = msg.Success ? int.Parse(msg.Value) : 0;

            if (hour >= 24)
                hour = 0;
            if (minutes >= 60)
                minutes = 0;
            if (seconds >= 60)
                seconds = 0;
            if (ms > 999)
                ms = 0;

            return new DateTime(year, month, day, hour, minutes, seconds, ms, calendar);
        }

        [JetBrains.Annotations.ContractAnnotation(@"null=>null;notnull=>notnull")]
        [JetBrains.Annotations.Pure]
        public static string Deserialize([JetBrains.Annotations.CanBeNull]string input)
        {
            if (input == null)
            {
                return null;
            }

            int n = input.Length;
            if (n == 0)
            {
                return string.Empty;
            }

            var result = new char[n];

            for (int i = 0; i < n; i++)
            {
                char c = input[i];
                switch (c)
                {
                    case 'ی':
                    case 'ى':
                        result[i] = 'ي';
                        break;

                    case 'ك':
                    case 'ﻙ':
                        result[i] = 'ک';
                        break;
                    default:
                        result[i] = c;
                        break;
                }
            }

            return new string(result);
        }

        [JetBrains.Annotations.Pure]
        public static char Deserialize(char input)
        {
            switch (input)
            {
                case 'ی':
                case 'ى':
                    return 'ي';

                case 'ك':
                case 'ﻙ':
                    return 'ک';

                default:
                    return input;
            }
        }

        /// <summary>
        ///     Serializes the specified input.
        /// </summary>
        /// <param name="input"> The input. </param>
        /// <returns> </returns>
        [JetBrains.Annotations.ContractAnnotation(@"null=>null;notnull=>notnull")]
        [JetBrains.Annotations.Pure, System.Diagnostics.Contracts.Pure, JetBrains.Annotations.MustUseReturnValue]
        public static string Serialize([JetBrains.Annotations.CanBeNull]string input)
        {
            return SerializeSafe(input);
        }

        /// <summary>
        ///     Serializes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        [JetBrains.Annotations.Pure]
        public static char Serialize(char input)
        {
            switch (input)
            {
                case 'ي':
                case 'ى':
                    return 'ی';

                case 'ﻙ':
                case 'ك':
                    return 'ک';

                default:
                    return input;
            }
        }

        /// <summary>
        ///     Converts the date to Persian date string (ex. 1390/04/21)
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        [JetBrains.Annotations.CanBeNull]
        public static string ToGregorianDate(this DateTime date)
        {
            if (date == DateTime.MinValue || date == DateTime.MaxValue)
            {
                return null;
            }

            return date.ToString(@"yyyy/MM/dd", CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///     Converts the nullable date to Persian date string and returns <c>null</c> for <c>null</c> values.(ex. 1390/04/21)
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        [JetBrains.Annotations.ContractAnnotation(@"null=>null;notnull=>notnull")]
        public static string ToGregorianDate(this DateTime? date)
        {
            return date == null ? null : date.Value.ToGregorianDate();
        }

        /// <summary>
        /// Converts the date to Persian date string (ex. 1390/04/21)
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="includeTime">if set to <c>true</c> [include time].</param>
        /// <returns></returns>
        public static string ToPersianDate(this DateTime date, bool includeTime = false)
        {
            if (date == DateTime.MinValue || date == DateTime.MaxValue)
            {
                return null;
            }

            int year = _PersianCalendar.GetYear(date);
            int month = _PersianCalendar.GetMonth(date);
            int day = _PersianCalendar.GetDayOfMonth(date);

            var result = string.Format(CultureInfo.InvariantCulture, "{0:0000}/{1:00}/{2:00}", year, month, day);
            if (includeTime) result += " " + date.ToString("HH:mm", CultureInfo.InvariantCulture);

            return result;
        }

        /// <summary>
        ///     Converts the nullable date to Persian date string and returns <c>null</c> for <c>null</c> values.(ex. 1390/04/21)
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        [JetBrains.Annotations.ContractAnnotation(@"null=>null;notnull=>notnull")]
        public static string ToPersianDate([JetBrains.Annotations.CanBeNull] this DateTime? date)
        {
            return date == null ? null : date.Value.ToPersianDate();
        }

        /// <summary>
        ///     Converts the date to Persian date
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>The Persian date</returns>
        public static DateTime ToPersianDateTime(this DateTime date)
        {
            if (date == DateTime.MinValue || date == DateTime.MaxValue)
            {
                return date;
            }

            int year = _PersianCalendar.GetYear(date);
            int month = _PersianCalendar.GetMonth(date);
            int day = _PersianCalendar.GetDayOfMonth(date);

            return new DateTime(year, month, day, date.Hour, date.Minute, date.Second, date.Millisecond, _PersianCalendar, date.Kind);
        }

        [JetBrains.Annotations.ContractAnnotation(@"null=>null;notnull=>notnull")]
        [JetBrains.Annotations.Pure, System.Diagnostics.Contracts.Pure]
        [JetBrains.Annotations.MustUseReturnValue]
        public static string MakeRtl(string input)
        {
            return input == null ? null : string.Format("\u202D\u200E{0}\u202C", input);
        }

        [JetBrains.Annotations.ContractAnnotation(@"null=>null;notnull=>notnull")]
        [JetBrains.Annotations.Pure, System.Diagnostics.Contracts.Pure]
        [JetBrains.Annotations.MustUseReturnValue]
        public static string ConvertNumbers(string input)
        {
            if (input == null)
                return null;
            int n = input.Length;
            if (n == 0)
                return string.Empty;
            var res = new char[input.Length];
            bool anyChange = false;
            for (int i = 0; i < n; i++)
            {
                char c = input[i];
                if (c >= '0' && c <= '9')
                {
                    res[i] = ConvertNumber(c);
                    anyChange = true;
                }
                else
                    res[i] = c;
            }
            return anyChange ? new string(res) : input;
        }

        #endregion

        #region Methods

        [JetBrains.Annotations.ContractAnnotation(@"null=>null;notnull=>notnull")]
        [JetBrains.Annotations.Pure, System.Diagnostics.Contracts.Pure]
        [JetBrains.Annotations.MustUseReturnValue]
        private static char ConvertNumber(char number)
        {
            switch (number)
            {
                case '0':
                    return '\u06F0';
                case '1':
                    return '\u06F1';
                case '2':
                    return '\u06F2';
                case '3':
                    return '\u06F3';
                case '4':
                    return '\u06F4';
                case '5':
                    return '\u06F5';
                case '6':
                    return '\u06F6';
                case '7':
                    return '\u06F7';
                case '8':
                    return '\u06F8';
                case '9':
                    return '\u06F9';
                default:
                    return number;
            }
        }

        [JetBrains.Annotations.ContractAnnotation(@"null=>null;notnull=>notnull")]
        [JetBrains.Annotations.Pure, System.Diagnostics.Contracts.Pure]
        private static string SerializeSafe([JetBrains.Annotations.CanBeNull] string input)
        {
            if (input == null)
            {
                return null;
            }

            int n = input.Length;
            if (n == 0)
            {
                return string.Empty;
            }

            var result = new char[n];

            bool anyChange = false;

            for (int i = 0; i < n; i++)
            {
                char c = input[i];
                switch (c)
                {
                    case 'ي':
                    case 'ى':
                        result[i] = 'ی';
                        anyChange = true;
                        break;
                    case 'ﻙ':
                    case 'ك':
                        result[i] = 'ک';
                        anyChange = true;
                        break;
                    default:
                        result[i] = c;
                        break;
                }
            }

            return anyChange ? new string(result) : input;
        }

        //#if UNSAFE
        //		[JetBrains.Annotations.ContractAnnotation("null=>null;notnull=>notnull")]
        //		[JetBrains.Annotations.Pure]
        //		private unsafe static string SerializeImpl(string input)
        //		{
        //			if (input == null)
        //			{
        //				return null;
        //			}

        //			int n = input.Length;
        //			if (n == 0)
        //			{
        //				return string.Empty;
        //			}

        //			if (n < 1024)
        //			{
        //				try
        //				{
        //					char* result = stackalloc char[n];

        //					for (int i = 0; i < n; i++)
        //					{
        //						char c = input[i];

        //						switch (c)
        //						{
        //							case 'ي':
        //							case 'ى':
        //								result[i] = 'ی';
        //								break;

        //							case 'ﻙ':
        //							case 'ك':
        //								result[i] = 'ک';
        //								break;

        //							default:
        //								result[i] = c;
        //								break;
        //						}
        //					}

        //					return new string(result, 0, n);
        //				}
        //				catch (StackOverflowException)
        //				{
        //					return SerializeSafe(input);
        //				}
        //			}

        //			return SerializeSafe(input);
        //		}
        //#endif

        #endregion

    }
}
