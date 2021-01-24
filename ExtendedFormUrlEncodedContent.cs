using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paya.Automation.Editor
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using JetBrains.Annotations;

    /// <summary>
    ///     A container for name/value tuples encoded using <c>application/x-www-form-urlencoded</c> <abbr>MIME</abbr> type.
    /// </summary>
    public sealed class ExtendedFormUrlEncodedContent : ByteArrayContent
    {
        private static readonly MediaTypeHeaderValue _MediaTypeHeaderValue = new MediaTypeHeaderValue(@"application/x-www-form-urlencoded");

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtendedFormUrlEncodedContent" /> class with a specific collection of
        ///     name/value pairs.
        /// </summary>
        /// <param name="nameValueCollection">A collection of name/value pairs.</param>
        public ExtendedFormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
            : base(GetContentByteArray(nameValueCollection))
        {
            this.Headers.ContentType = _MediaTypeHeaderValue;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtendedFormUrlEncodedContent" /> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public ExtendedFormUrlEncodedContent(IDictionary<string, string> dictionary)
            : this((IEnumerable<KeyValuePair<string, string>>)dictionary)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtendedFormUrlEncodedContent" /> class.
        /// </summary>
        /// <param name="nameValues">The name values.</param>
        public ExtendedFormUrlEncodedContent(params KeyValuePair<string, string>[] nameValues)
            : this((IEnumerable<KeyValuePair<string, string>>)nameValues)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtendedFormUrlEncodedContent" /> class.
        /// </summary>
        /// <param name="nameValues">The name values.</param>
        public ExtendedFormUrlEncodedContent(IEnumerable<Tuple<string, string>> nameValues)
            : this(nameValues.Where(x => x != null).Select(x => new KeyValuePair<string, string>(x.Item1, x.Item2)))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtendedFormUrlEncodedContent" /> class.
        /// </summary>
        /// <param name="nameValues">The name values.</param>
        public ExtendedFormUrlEncodedContent(params Tuple<string, string>[] nameValues)
            : this((IEnumerable<Tuple<string, string>>)nameValues)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtendedFormUrlEncodedContent" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public ExtendedFormUrlEncodedContent(object data)
            : this(Utility.GetUrlEncodedData(data))
        {
        }

        #endregion

        #region Public Methods and Operators

        public static string EscapeDataString(string data)
        {
            return EscapeDataString(data, 65510);
        }

        #endregion

        #region Methods

        [System.Diagnostics.Contracts.Pure]
        private static string EscapeDataString(string data, int chunkLimit)
        {
            if (string.IsNullOrEmpty(data))
                return string.Empty;

            if (data.Length < chunkLimit)
                return Uri.EscapeDataString(data);

            var sb = new StringBuilder();
            int loops = (int)Math.Ceiling(data.Length / (double)chunkLimit);

            for (int i = 0; i < loops; i++)
            {
                sb.Append(Uri.EscapeDataString(i < loops - 1 ? data.Substring(chunkLimit * i, chunkLimit) : data.Substring(chunkLimit * i)));
            }

            return sb.ToString();
        }

        [System.Diagnostics.Contracts.Pure]
        private static byte[] GetContentByteArray([NotNull] IEnumerable<KeyValuePair<string, string>> nameValueCollection)
        {
            if (nameValueCollection == null)
                throw new ArgumentNullException("nameValueCollection");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            var stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> keyValuePair in nameValueCollection)
            {
                if (stringBuilder.Length > 0)
                    stringBuilder.Append('&');
                stringBuilder.Append(EscapeDataString(keyValuePair.Key)).Append('=').Append(EscapeDataString(keyValuePair.Value));
            }

            return Encoding.UTF8.GetBytes(stringBuilder.ToString());
        }

        #endregion
    }
}
