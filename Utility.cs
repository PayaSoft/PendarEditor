using System;
using System.Collections.Generic;

namespace Paya.Automation.Editor
{
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Security;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NLog;

    using TaskEx = System.Threading.Tasks.Task;

    internal static class Utility
    {
        #region Static Fields

        private static readonly char[] _SortedInvalidFileNameChars = Path.GetInvalidFileNameChars().OrderBy(x => x).ToArray();

        private static readonly char[] _SortedInvalidPathChars = Path.GetInvalidPathChars().OrderBy(x => x).ToArray();

        private static readonly Logger _Logger = LogManager.GetCurrentClassLogger();

        private static readonly Logger _NetLogger = LogManager.GetLogger("NETWORK");

        private static readonly ConcurrentDictionary<Uri, HttpClient> _HttpClients = new ConcurrentDictionary<Uri, HttpClient>();

        #endregion

        #region Public Methods and Operators

        internal static void CheckDirectoryOfFileExists([NotNull] string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException("filePath");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            var dir = Path.GetDirectoryName(filePath);
            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        [ContractAnnotation(@"fileName:null=>null;fileName:notnull=>notnull")]
        internal static string ReplaceInvalidFileNameChars([CanBeNull] string fileName, char replaceWithChar = ' ')
        {
            if (fileName == null)
                return null;

            var newFileName = new char[fileName.Length];
            for (int i = 0; i < fileName.Length; i++)
            {
                char c = fileName[i];
                bool isInvalid = Array.BinarySearch(_SortedInvalidFileNameChars, c) >= 0;
                newFileName[i] = isInvalid ? replaceWithChar : c;
            }

            return new string(newFileName);
        }

        [CanBeNull]
        internal static string ReplaceInvalidPathChars([CanBeNull] string fileName, char replaceWithChar = ' ')
        {
            if (fileName == null)
                return null;
            var newFileName = new char[fileName.Length];
            for (int i = 0; i < fileName.Length; i++)
            {
                char c = fileName[i];
                bool isInvalid = Array.BinarySearch(_SortedInvalidPathChars, c) >= 0;
                newFileName[i] = isInvalid ? replaceWithChar : c;
            }
            return new string(newFileName);
        }

        #endregion

        #region Methods

        internal static bool FixBase64SpaceChars(ref string xamlDoc)
        {
            if (string.IsNullOrWhiteSpace(xamlDoc))
                return true;

            try
            {
                var doc = XDocument.Parse(xamlDoc);
                if (doc.Root != null)
                    foreach (var e in doc.Root.Descendants(XName.Get("ImageInline", @"clr-namespace:Telerik.Windows.Documents.Model;assembly=Telerik.Windows.Documents")))
                    {
                        var attr = e.Attribute("RawData");
                        if (attr == null)
                            continue;
                        string val = attr.Value;
                        if (string.IsNullOrWhiteSpace(val))
                            continue;
                        attr.Value = val.Replace(' ', '+');
                    }
                xamlDoc = doc.ToString(SaveOptions.DisableFormatting);
                return true;
            }
            catch (Exception exp)
            {
                if (_Logger.IsWarnEnabled)
                    _Logger.Warn(exp, "Error in FixBase64SpaceChars");
                return false;
            }
        }


        [SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
        [ContractAnnotation("data:null=>null")]
        [CanBeNull]
        internal static T GetJsonObject<T>(this JObject data, [CanBeNull] string path)
            where T : JToken
        {
            var items = path != null ? path.Split('.') : new string[0];
            JToken t = data;
            if (t == null)
                return default(T);
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                for (; ; )
                {
                    var iStart = item.LastIndexOf('[');
                    if (iStart < 0)
                    {
                        break;
                    }

                    var iEnd = item.LastIndexOf(']');
                    if (iEnd < 0 || iEnd <= iStart)
                    {
                        break;
                    }

                    var indexStr = item.Substring(iStart + 1, iEnd - iStart - 1);
                    int index;
                    if (!int.TryParse(indexStr, out index))
                        break;
                    var a = t as JArray;
                    if (a == null)
                        return default(T);
                    t = a[index];
                    item = item.Remove(iStart);
                }

                var o = t as JObject;
                if (o == null)
                    return default(T);
                t = o[item];
            }

            return t as T;
        }

        [SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
        [ContractAnnotation("data:null=>null")]
        [CanBeNull]
        internal static T GetJsonValue<T>(this JObject data, string path)
        {
            JToken t = data;
            if (t == null)
                return default(T);

            try
            {

                var items = path != null ? path.Split('.') : new string[0];

                for (int i = 0; i < items.Length; i++)
                {
                    var item = items[i];
                    for (; ; )
                    {
                        var iStart = item.LastIndexOf('[');
                        if (iStart < 0)
                        {
                            break;
                        }

                        var iEnd = item.LastIndexOf(']');
                        if (iEnd < 0 || iEnd <= iStart)
                        {
                            break;
                        }

                        var indexStr = item.Substring(iStart + 1, iEnd - iStart - 1);
                        int index;
                        if (!int.TryParse(indexStr, out index))
                            break;
                        var a = t as JArray;
                        if (a == null || a.Type == JTokenType.Null)
                            return default(T);
                        t = a[index];
                        item = item.Remove(iStart);
                    }

                    var o = t as JObject;
                    if (o == null || o.Type == JTokenType.Null)
                        return default(T);
                    t = o[item];
                }

                return t != null ? t.Value<T>() : default(T);
            }
            catch (Exception exp)
            {
                if (_Logger.IsErrorEnabled)
                    _Logger.Error(exp, "Error while getting JSON value from data path {0}", path);

                return default(T);
            }
        }

        [CanBeNull, ContractAnnotation("response:null=>null")]
        internal static JToken GetResponseResult(JToken response)
        {
            if (response == null)
                return null;

            var status = response is JObject ? response["Status"] as JObject : null;
            if (status != null)
            {
                if (!status["Succeed"].Value<bool>())
                    throw new InvalidOperationException(status["Message"].Value<string>());
            }

            return response;
        }

        [CanBeNull, ContractAnnotation("response:null=>null")]
        internal static JToken GetStandardResponseResult(JToken response)
        {
            if (response == null)
                return null;

            if (!(response is JObject))
                throw new InvalidOperationException("Invalid response. Expected object and got array.");

            //var status = response["Status"] as JObject;
            //if (status == null)
            //    throw new InvalidOperationException("Invalid response. No Status defined.");

            //if (!status["Succeed"].Value<bool>())
            //{
            //    if (status["IsLoggedIn"].Value<bool?>() == false)
            //    {
            //        throw new SecurityException(Resources.NotAuthenticatedErrorMessage);
            //    }

            //    throw new InvalidOperationException(status["Message"].Value<string>() ?? "خطا");
            //}

            //return response["Data"];

            return response;
        }

        internal static IEnumerable<KeyValuePair<string, string>> GetUrlEncodedData([CanBeNull] object obj)
        {
            return obj == null ? Enumerable.Empty<KeyValuePair<string, string>>() : GetUrlEncodedData(string.Empty, obj, CultureInfo.InvariantCulture);
        }

        internal static async Task<object> PostRequestAndDownloadAsync([NotNull] string baseUrl, string url, [CanBeNull] HttpContent content = null, [CanBeNull] string token = null, [CanBeNull] CancellationTokenSource cancellationTokenSource = null)
        {
            if (_NetLogger.IsTraceEnabled)
                _Logger.Trace("Starting POST {0}/{1}", baseUrl, url);

            var baseAddress = new Uri(baseUrl);
            {
                var w = _HttpClients.GetOrAdd(baseAddress, key => new HttpClient { BaseAddress = key });
                {
                    // Set Authentication Token
                    if (!string.IsNullOrWhiteSpace(token))
                        w.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    w.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");

                    using (var result = await (cancellationTokenSource != null ? w.PostAsync(url, content, cancellationTokenSource.Token) : w.PostAsync(url, content)))
                    {
                        if (_NetLogger.IsDebugEnabled)
                            _Logger.Debug("Completed POST {0}/{1} -> StatusCode = {2}", baseUrl, url, result.StatusCode);

                        result.EnsureSuccessStatusCode();

                        bool isJson = result.Content.Headers.ContentType.MediaType.Equals(@"application/json", StringComparison.OrdinalIgnoreCase);

                        if (isJson)
                        {
                            var resultObject = JsonConvert.DeserializeObject<JToken>(await result.Content.ReadAsStringAsync());

                            if (resultObject == null)
                                throw new InvalidOperationException("The response is empty.");

                            var status = resultObject["Status"];

                            if (status == null)
                                throw new InvalidOperationException("The response JSON is not valid.");

                            if (!status["Succeed"].Value<bool>())
                                throw new InvalidOperationException((status["Message"] != null ? status["Message"].Value<string>() : null) ?? "The response JSON is not valid.");

                            return resultObject["Data"];
                        }

                        if (!string.IsNullOrWhiteSpace(result.Content.Headers.ContentDisposition.FileName))
                        {
                            return await result.Content.ReadAsByteArrayAsync();
                        }

                        return await result.Content.ReadAsStringAsync();
                    }
                }
            }
        }

        internal static async Task<byte[]> HttpGetResponseAsync([NotNull] Uri uri, [CanBeNull] string token, [CanBeNull] CancellationTokenSource cancellationTokenSource = null)
        {
            if (_NetLogger.IsTraceEnabled)
                _Logger.Trace("Starting GET {0}", uri);

            var baseUri = new Uri(uri.GetComponents(UriComponents.HostAndPort | UriComponents.SchemeAndServer | UriComponents.UserInfo, UriFormat.SafeUnescaped), UriKind.Absolute);
            using (var handler = new HttpClientHandler { })
            {
                using (var w = new HttpClient(handler) { BaseAddress = baseUri })
                {
                    // Set Authentication Token
                    if (!string.IsNullOrWhiteSpace(token))
                        w.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    w.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");

                    using (var result = await (cancellationTokenSource != null ? w.GetAsync(uri, cancellationTokenSource.Token) : w.GetAsync(uri)))
                    {
                        if (_NetLogger.IsDebugEnabled)
                            _Logger.Debug("Completed GET {0} -> StatusCode = {1}", uri, result.StatusCode);

                        result.EnsureSuccessStatusCode();
                        var data = await result.Content.ReadAsByteArrayAsync();
                        return data;
                    }
                }
            }
        }

        internal static async Task<byte[]> HttpGetResponseAsync([NotNull] string baseUrl, string url, [CanBeNull] string token)
        {
            if (_NetLogger.IsTraceEnabled)
                _Logger.Trace("Starting GET {0}/{1}", baseUrl, url);

            var baseAddress = new Uri(baseUrl);
            using (var handler = new HttpClientHandler { })
            {
                using (var w = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    // Set Authentication Token
                    if (!string.IsNullOrWhiteSpace(token))
                        w.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    w.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");

                    using (var result = await w.GetAsync(url))
                    {
                        if (_NetLogger.IsDebugEnabled)
                            _Logger.Debug("Completed GET {0}/{1} -> StatusCode = {2}", baseUrl, url, result.StatusCode);

                        result.EnsureSuccessStatusCode();
                        var data = await result.Content.ReadAsByteArrayAsync();
                        return data;
                    }
                }
            }
        }

        internal static async Task<T> HttpGetResponseAsync<T>([NotNull] string baseUrl, string url, [CanBeNull] string token)
        {
            if (_NetLogger.IsTraceEnabled)
                _Logger.Trace("Starting GET {0}/{1}", baseUrl, url);

            var baseAddress = new Uri(baseUrl);
            using (var handler = new HttpClientHandler { })
            {
                using (var w = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    // Set Authentication Token
                    if (!string.IsNullOrWhiteSpace(token))
                        w.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    w.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");

                    using (var result = await w.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                    {
                        if (_NetLogger.IsDebugEnabled)
                            _Logger.Debug("Completed GET {0}/{1} -> StatusCode = {2}", baseUrl, url, result.StatusCode);

                        result.EnsureSuccessStatusCode();
                        var data = await result.Content.ReadAsStringAsync();
                        var r = await TaskEx.Run(() => JsonConvert.DeserializeObject<T>(data));
                        return r;
                    }
                }
            }
        }

        internal static async Task<string> HttpGetResponseStringAsync([NotNull] string baseUrl, string url, [CanBeNull] IEnumerable<KeyValuePair<string, string>> cookies)
        {
            if (_NetLogger.IsTraceEnabled)
                _Logger.Trace("Starting GET {0}/{1}", baseUrl, url);

            var baseAddress = new Uri(baseUrl);
            var w = _HttpClients.GetOrAdd(baseAddress, key => new HttpClient { BaseAddress = key });
            {
                {
                    w.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");

                    using (var result = await w.GetAsync(url))
                    {
                        if (_NetLogger.IsDebugEnabled)
                            _Logger.Debug("Completed GET {0}/{1} -> StatusCode = {2}", baseUrl, url, result.StatusCode);

                        result.EnsureSuccessStatusCode();
                        var data = await result.Content.ReadAsStringAsync();
                        return data;
                    }
                }
            }
        }

        internal static async Task<JToken> HttpPostRequestAsync(string baseUrl, string url, HttpContent content, string token, [CanBeNull] CancellationTokenSource cancellationTokenSource = null)
        {
            return await HttpPostRequestAsync<JToken>(baseUrl, url, content, token, cancellationTokenSource);
        }

        internal static async Task<T> HttpPostRequestAsync<T>([NotNull] string baseUrl, string url, HttpContent content, [CanBeNull] string token, [CanBeNull] CancellationTokenSource cancellationTokenSource = null)
        {
            if (_NetLogger.IsTraceEnabled)
                _Logger.Trace("Starting POST {0}/{1}", baseUrl, url);

            var baseAddress = new Uri(baseUrl);

            var w = _HttpClients.GetOrAdd(baseAddress, key => new HttpClient { BaseAddress = key });
            {
                {
                    // Set Authentication Token
                    if (!string.IsNullOrWhiteSpace(token))
                        w.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    w.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");

                    using (var result = await (cancellationTokenSource != null ? w.PostAsync(url, content, cancellationTokenSource.Token) : w.PostAsync(url, content)))
                    {
                        if (_NetLogger.IsDebugEnabled)
                            _Logger.Debug("Completed POST {0}/{1} -> StatusCode = {2}", baseUrl, url, result.StatusCode);

                        result.EnsureSuccessStatusCode();
                        var data = await result.Content.ReadAsStringAsync();
                        T r;
                        if (cancellationTokenSource != null)
                            r = await TaskEx.Run(() => JsonConvert.DeserializeObject<T>(data), cancellationTokenSource.Token);
                        else r = JsonConvert.DeserializeObject<T>(data);
                        return r;
                    }
                }
            }
        }

        internal static async Task<JObject> HttpPatchRequestAsync<T>([NotNull] string baseUrl, string url, HttpContent content, [CanBeNull] string token, [CanBeNull] CancellationTokenSource cancellationTokenSource = null)
        {
            if (_NetLogger.IsTraceEnabled)
                _Logger.Trace("Starting PUT {0}/{1}", baseUrl, url);

            var baseAddress = new Uri(baseUrl);

            var w = _HttpClients.GetOrAdd(baseAddress, key => new HttpClient { BaseAddress = key });
            {
                {
                    // Set Authentication Token
                    if (!string.IsNullOrWhiteSpace(token))
                        w.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    w.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");

                    var request = new HttpRequestMessage(new HttpMethod("PATCH"), url);
                    request.Content = content;

                    using (var result = await (cancellationTokenSource != null ? w.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationTokenSource.Token) : w.SendAsync(request)))
                    {
                        if (_NetLogger.IsDebugEnabled)
                            _Logger.Debug("Completed Put {0}/{1} -> StatusCode = {2}", baseUrl, url, result.StatusCode);

                        result.EnsureSuccessStatusCode();

                        var response = new JObject();

                        if (result.StatusCode == HttpStatusCode.NoContent || result.StatusCode == HttpStatusCode.OK)
                        {
                            var okStatus = new JObject
                            {
                                { "Succeed", true }
                            };

                            response.Add("Status", okStatus);

                            return response;
                        }

                        var errorStatus = new JObject
                            {
                                { "Message", "Something Went Wrong While Update Document." }
                            };

                        response.Add("Status", errorStatus);

                        return response;

                    }
                }
            }
        }

        internal static async Task<byte[]> HttpPostRequestBinaryAsync([NotNull] string baseUrl, string url, HttpContent content, [CanBeNull] string token, [CanBeNull] CancellationTokenSource cancellationTokenSource = null)
        {
            if (_NetLogger.IsTraceEnabled)
                _Logger.Trace("Starting POST {0}/{1}", baseUrl, url);

            var baseAddress = new Uri(baseUrl);
            var w = _HttpClients.GetOrAdd(baseAddress, key => new HttpClient { BaseAddress = key });
            {
                {
                    // Set Authentication Token
                    if (!string.IsNullOrWhiteSpace(token))
                        w.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    w.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");

                    using (var result = await (cancellationTokenSource != null ? w.PostAsync(url, content, cancellationTokenSource.Token) : w.PostAsync(url, content)))
                    {
                        if (_NetLogger.IsDebugEnabled)
                            _Logger.Debug("Completed POST {0}/{1} -> StatusCode = {2}", baseUrl, url, result.StatusCode);

                        result.EnsureSuccessStatusCode();

                        return await result.Content.ReadAsByteArrayAsync();
                    }
                }
            }
        }

        internal static T? TryParseEnum<T>(object value)
            where T : struct
        {
            T r;
            return Enum.TryParse(Convert.ToString(value), true, out r) ? (T?)r : null;
        }

        [ContractAnnotation(@"input:null=>null;input:notnull=>notnull")]
        public static string Unescape(string input, bool convertPlusToSpace = false, Encoding encoding = null)
        {
            // the HttpUtility.UrlDecode removes '+' chars!
            // the Uri.UnescapeDataString does not decode Unicode chars: %u062A

            if (string.IsNullOrEmpty(input))
                return input;

            bool anyUnicodeChar = false;

            bool anyChange = false;
            int n = input.Length;
            var result = new int[n];
            int m = 0;
            for (int i = 0; i < n; i++)
            {
                char c = input[i];
                if (convertPlusToSpace && c == '+')
                {
                    result[m++] = ' ';
                    anyChange = true;
                }
                else
                {
                    if (c == '%' && i < n - 2)
                    {
                        char c1 = input[i + 1];
                        char c2 = input[i + 2];

                        if (c1 == 'u' && i < n - 5)
                        {
                            char c3 = input[i + 3];
                            char c4 = input[i + 4];
                            char c5 = input[i + 5];

                            int ciu2 = ConvertCharToHexNumber(c2), ciu3 = ConvertCharToHexNumber(c3), ciu4 = ConvertCharToHexNumber(c4), ciu5 = ConvertCharToHexNumber(c5);

                            if (ciu2 >= 0 && ciu3 >= 0 && ciu4 >= 0 && ciu5 >= 0)
                            {
                                int num = (ciu2 << 12) + (ciu3 << 8) + (ciu4 << 4) + ciu5;
                                result[m++] = num;
                                i += 5;
                                anyChange = true;
                                if (num > byte.MaxValue)
                                    anyUnicodeChar = true;
                            }
                            else
                            {
                                result[m++] = c;
                                //result[m++] = c1;
                                //result[m++] = c2;
                                //result[m++] = c3;
                                //result[m++] = c4;
                                //result[m++] = c5;
                                //i += 5;
                            }
                        }
                        else
                        {
                            int ci1 = ConvertCharToHexNumber(c1), ci2 = ConvertCharToHexNumber(c2);

                            if (ci1 >= 0 && ci2 >= 0)
                            {
                                int num = (ci1 << 4) + ci2;

                                result[m++] = num;
                                i += 2;
                                anyChange = true;
                            }
                            else
                            {
                                result[m++] = c;
                                //result[m++] = c1;
                                //result[m++] = c2;
                                //i += 2;
                            }
                        }
                    }
                    else
                    {
                        result[m++] = c;
                    }
                }
            }

            if (!anyChange)
                return input;

            if (!anyUnicodeChar)
            {
                var bytes = result.Take(m).Select(x => (byte)x).ToArray();
                if (encoding == null)
                    encoding = Encoding.UTF8;
                return encoding.GetString(bytes);
            }

            var s = result.Take(m).Aggregate(new StringBuilder(), (sb, i) => sb.Append(char.ConvertFromUtf32(i)), sb => sb.ToString());
            return s;
        }

        private static int ConvertCharToHexNumber(char ch)
        {
            if (ch >= '0' && ch <= '9')
                return ch - '0';
            if (ch >= 'A' && ch <= 'F')
                return (ch - 'A') + 10;
            if (ch >= 'a' && ch <= 'f')
                return (ch - 'a') + 10;
            return -1;
        }

        private static IEnumerable<KeyValuePair<string, string>> GetUrlEncodedData(string name, [CanBeNull] object obj, IFormatProvider formatProvider)
        {
            if (obj == null)
                yield break;

            var objType = obj.GetType();

            var type = Nullable.GetUnderlyingType(objType) ?? objType;

            if (obj is string || typeof(IConvertible).IsAssignableFrom(type))
                yield return new KeyValuePair<string, string>(name, Convert.ToString(obj, formatProvider));
            else
            {
                var items = obj as IEnumerable;
                if (items != null)
                {
                    int counter = 0;
                    foreach (object item in items)
                    {
                        yield return new KeyValuePair<string, string>(string.Format(formatProvider, "{0}[{1}]", name, counter++), Convert.ToString(item, formatProvider));
                    }
                }
                else
                {
                    var props = objType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy).Where(p => p.CanRead);

                    foreach (var p in props)
                    {
                        string pName = string.IsNullOrEmpty(name) ? p.Name : string.Format(formatProvider, "{0}[{1}]", name, p.Name);

                        var value = p.GetValue(obj, null);
                        if (value == null)
                            yield return new KeyValuePair<string, string>(pName, string.Empty);
                        else
                        {
                            var valueType = Nullable.GetUnderlyingType(value.GetType()) ?? objType;

                            if (value is string || typeof(IConvertible).IsAssignableFrom(valueType))
                            {
                                yield return new KeyValuePair<string, string>(pName, Convert.ToString(value, formatProvider));
                            }

                            else
                            {
                                var enumerable = value as IEnumerable;
                                if (enumerable != null)
                                {
                                    int counter = 0;
                                    foreach (var x in from object item in enumerable
                                                      from x in GetUrlEncodedData(string.Format(formatProvider, "{0}[{1}]", pName, counter++), item, formatProvider)
                                                      select x)
                                    {
                                        yield return x;
                                    }
                                }
                                else
                                {
                                    foreach (var x in GetUrlEncodedData(pName, value, formatProvider))
                                        yield return x;
                                }
                            }
                        }
                    }
                }
            }
        }

        [NotNull, ItemNotNull]
        public static async Task<byte[]> ReadAllBytesAsync([NotNull] string fileName, [CanBeNull] CancellationTokenSource cancellationTokenSource = null)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            Contract.EndContractBlock();

            using (var fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return await ReadAllBytesAsync(fs, cancellationTokenSource: cancellationTokenSource);
            }
        }

        [NotNull, ItemNotNull]
        public static async Task<byte[]> ReadAllBytesAsync([NotNull] this Stream stream, int bufferSize = 4096, [CanBeNull] CancellationTokenSource cancellationTokenSource = null)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            Contract.EndContractBlock();

            if (stream.CanSeek)
            {
                var buffer = new byte[stream.Length];
                int res = await (cancellationTokenSource == null ? stream.ReadAsync(buffer, 0, buffer.Length) : stream.ReadAsync(buffer, 0, buffer.Length, cancellationTokenSource.Token));
                if (res != buffer.Length)
                    Array.Resize(ref buffer, res);
                return buffer;
            }
            else
            {
                byte[] buffer = new byte[bufferSize];

                var result = new List<byte>();

                int readBytes;
                do
                {
                    readBytes = await (cancellationTokenSource != null ? stream.ReadAsync(buffer, 0, buffer.Length, cancellationTokenSource.Token) : stream.ReadAsync(buffer, 0, buffer.Length));
                    if (readBytes > 0)
                    {
                        result.AddRange(buffer.Take(readBytes));
                    }
                }
                while (readBytes >= buffer.Length);

                return result.ToArray();
            }
        }

        public static async Task WriteAllBytesAsync([NotNull] string filePath, [CanBeNull] byte[] data, int bufferSize = 4096, [CanBeNull] CancellationTokenSource cancellationTokenSource = null)
        {
            if (filePath == null)
                throw new ArgumentNullException("filePath");

            Contract.EndContractBlock();

            if (data == null)
                return;

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read, bufferSize, true))
            {
                if (cancellationTokenSource != null)
                    await stream.WriteAsync(data, 0, data.Length, cancellationTokenSource.Token);
                else
                    await stream.WriteAsync(data, 0, data.Length);
            }
        }

        internal static string NormalizeUrl(string url)
        {
            try
            {
                Uri uri;
                return !Uri.TryCreate(url, UriKind.Absolute, out uri) || uri == null ? url : uri.GetComponents(UriComponents.AbsoluteUri, UriFormat.SafeUnescaped);
            }
            catch (Exception exp)
            {
                if (_Logger.IsWarnEnabled)
                    _Logger.Warn(exp, "Error while normalizing URL {0}", url);
                return url;
            }
        }




        #endregion



    }
}
