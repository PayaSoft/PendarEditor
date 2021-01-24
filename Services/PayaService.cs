namespace Paya.Automation.Editor.Services
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Shell;

    using JetBrains.Annotations;
    using NLog;

    using Cryptography;
    using Models;
    using System.Net.Http;
    using Newtonsoft.Json.Linq;

    [ServiceBehavior(Namespace = Internal.DefaultNamespace,
        IncludeExceptionDetailInFaults = true,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class PayaService : IPayaService, IPolicyRetriever
    {
        #region Static Fields

        private static readonly HashSet<BaseUrlMessageSerialTuple> _LoadingMessages = new HashSet<BaseUrlMessageSerialTuple>();

        private static readonly Logger _Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        private static bool IsOptions
        {
            get
            {
                var ctx = WebOperationContext.Current;
                return ctx != null && "OPTIONS".Equals(ctx.IncomingRequest.Method, StringComparison.OrdinalIgnoreCase);
            }
        }

        #endregion

        #region Public Methods and Operators

        [NotNull]
        public Stream GetFavicon()
        {
            if (_Logger.IsTraceEnabled)
                _Logger.Trace("GetFavicon");

            if (WebOperationContext.Current != null)
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = "image/x-icon";
            }

            var ms = new MemoryStream();
            Resources.Favicon.Save(ms);
            return ms;
        }

        /// <summary>Enables the cross domain policy of the WCF service.</summary>
        [NotNull]
        public Stream GetFlashPolicy()
        {
            if (_Logger.IsTraceEnabled)
                _Logger.Trace("GetFlashPolicy");

            if (WebOperationContext.Current != null)
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = @"application/xml";
            }

            return new MemoryStream(Encoding.UTF8.GetBytes(Resources.crossdomain), false);
        }

        /// <summary>Enables the cross domain policy of the WCF service.</summary>
        [NotNull]
        public Stream GetSilverlightPolicy()
        {
            if (_Logger.IsTraceEnabled)
                _Logger.Trace("GetSilverlightPolicy");

            if (WebOperationContext.Current != null)
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = @"application/xml";
            }

            return new MemoryStream(Encoding.UTF8.GetBytes(Resources.clientaccesspolicy), false);
        }

        public void Ping()
        {
            if (IsOptions)
            {
                return;
            }

            if (_Logger.IsDebugEnabled)
                _Logger.Debug("Ping");
        }


       public void OpenBody(string baseUrl, int storeIndex, string messageId, int messageSerial, bool insertHeader, bool insertSigns, bool insertSignImage, bool insertCopyText, bool insertRemarks, string cipher, Dictionary<string, string> encryptedCookies)
        {
            if (IsOptions)
            {
                return;
            }

            var loadingItem = new BaseUrlMessageSerialTuple(baseUrl, messageSerial);
            lock (_LoadingMessages)
            {
                if (_Logger.IsDebugEnabled)
                    _Logger.Debug("Checking is loading {0}", messageSerial);

                if (_LoadingMessages.Contains(loadingItem))
                {
                    if (_Logger.IsDebugEnabled)
                        _Logger.Debug("Already is loading {0}", messageSerial);

                    return;
                }

                _LoadingMessages.Add(loadingItem);

                if (_Logger.IsDebugEnabled)
                    _Logger.Debug("Loading {0}", messageSerial);
            }

            try
            {

                if (_Logger.IsDebugEnabled)
                    _Logger.Debug("Begin OpenBody");

                var cookies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                if (encryptedCookies != null)
                {
                    foreach (var item in encryptedCookies)
                        cookies[item.Key] = Crypto.DecryptAndVerify(item.Value, cipher, false);
                }

                if (baseUrl != null)
                {
                    var u = new Uri(baseUrl, UriKind.RelativeOrAbsolute);
                    Application.SetCookie(u, cookies.Aggregate(new StringBuilder(), (sb, c) => (sb.Length > 0 ? sb.Append(';') : sb).Append(c.Key).Append('=').Append(c.Value), sb => sb.ToString()));

                    if (_Logger.IsTraceEnabled)
                        _Logger.Trace("Cookies set");

                    //try
                    //{
                    //    if (_Logger.IsDebugEnabled)
                    //    {
                    //        _Logger.Debug("Initializing the updater.");
                    //    }

                    //    App.ClientUpdaterFactory.CreateUpdater(baseUrl);
                    //}
                    //catch (Exception exp)
                    //{
                    //    if (_Logger.IsWarnEnabled)
                    //    {
                    //        _Logger.Warn(exp, "Error while setting updater parameters.");
                    //    }
                    //}
                }

                var cancelationToken = new CancellationTokenSource();

                var sessionData = new MessageSessionData(baseUrl, storeIndex, messageId, messageSerial, cookies);


                var m = new WordEditing.WordEditorManager(sessionData) { InsertHeader = insertHeader, InsertSigns = insertSigns, InsertSignImage = insertSignImage, InsertCopyText = insertCopyText, InsertRemarks = insertRemarks };

                Application.Current.Dispatcher.BeginInvoke(new Func<Task>(async () =>
                {
                    try
                    {
                        await m.LaunchAsync(cancelationToken);
                    }
                    catch (Exception exp)
                    {
                        if (_Logger.IsErrorEnabled)
                            _Logger.Error(exp, "Loading error for {0}", messageSerial);
                    }
                    finally
                    {
                        lock (_LoadingMessages)
                        {
                            _LoadingMessages.Remove(loadingItem);

                            if (_Logger.IsDebugEnabled)
                                _Logger.Debug("Loading completed {0}", messageSerial);
                        }
                    }
                }));

            }
            catch
            {
                lock (_LoadingMessages)
                {
                    _LoadingMessages.Remove(loadingItem);

                    if (_Logger.IsDebugEnabled)
                        _Logger.Debug("Loading errors {0}", messageSerial);
                }

                throw;
            }
        }

        public void Print(string baseUrl, int storeIndex, string messageId, int messageSerial, bool insertHeader, bool insertSigns, bool insertSignImage, bool insertCopyText, bool insertRemarks, bool withPreview, string cipher, Dictionary<string, string> encryptedCookies)
        {
            if (IsOptions)
            {
                return;
            }

            if (_Logger.IsDebugEnabled)
                _Logger.Debug("Begin Print");

            var cookies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (encryptedCookies != null)
            {
                foreach (var item in encryptedCookies)
                    cookies[item.Key] = Crypto.DecryptAndVerify(item.Value, cipher, false);
            }

            if (baseUrl != null)
            {
                var u = new Uri(baseUrl, UriKind.RelativeOrAbsolute);
                Application.SetCookie(u, cookies.Aggregate(new StringBuilder(), (sb, c) => (sb.Length > 0 ? sb.Append(';') : sb).Append(c.Key).Append('=').Append(c.Value), sb => sb.ToString()));
            }

            if (_Logger.IsTraceEnabled)
                _Logger.Trace("Cookies set");

            var cancelationToken = new CancellationTokenSource();

            var sessionData = new MessageSessionData(baseUrl, storeIndex, messageId, messageSerial, cookies);

            var m = new WordEditing.WordEditorManager(sessionData) { InsertHeader = insertHeader, InsertSigns = insertSigns, InsertSignImage = insertSignImage, InsertCopyText = insertCopyText, InsertRemarks = insertRemarks };

            Application.Current.Dispatcher.BeginInvoke(new Func<Task>(async () =>
            {
                try
                {
                    await m.PrintAsync(withPreview, cancelationToken);
                }
                catch (Exception exp)
                {
                    if (_Logger.IsErrorEnabled)
                        _Logger.Error(exp, "Error while printing");
                }
            })).Completed += (s, e) => InsertPrintLog(sessionData, cancelationToken);
        }

        private static async void InsertPrintLog(MessageSessionData sessionData, CancellationTokenSource cancelationToken)
        {
            try
            {
                // { storeIndex: window.top.storeIndex, folderId: LastFolderId, messageId: LastmessageId }

                var pdata = new Dictionary<string, string>();

                pdata["storeIndex"] = Convert.ToString(sessionData.StoreIndex);
                pdata["messageId"] = Convert.ToString(sessionData.MessageId);

                var json = await Utility.HttpPostRequestAsync<JObject>(sessionData.BaseUrl, @"/Crm/Table_AUT_PrintLog/InsertPrintLog", new FormUrlEncodedContent(pdata), sessionData.Cookies, cancelationToken);
                if (json != null && json["Status"] != null && !json["Status"]["Succeed"].Value<bool>())
                {
                    if (_Logger.IsErrorEnabled)
                        _Logger.Error("Server Error: {0}", json["Status"]["Message"].Value<string>());
                }
            }
            catch (Exception exp)
            {
                if (_Logger.IsErrorEnabled)
                    _Logger.Error(exp, "Error while logging the print operation");
            }
        }

        #endregion

        [Serializable]
        private struct BaseUrlMessageSerialTuple : IEquatable<BaseUrlMessageSerialTuple>
        {
            #region Fields

            private readonly string _baseUrl;
            private readonly int _messageSerial;

            #endregion

            #region Constructors and Destructors

            internal BaseUrlMessageSerialTuple(string baseUrl, int messageSerial) : this()
            {
                this._baseUrl = Utility.NormalizeUrl(baseUrl);
                this._messageSerial = messageSerial;
            }

            #endregion

            #region Public Methods and Operators

            public static bool operator ==(BaseUrlMessageSerialTuple left, BaseUrlMessageSerialTuple right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(BaseUrlMessageSerialTuple left, BaseUrlMessageSerialTuple right)
            {
                return !(left == right);
            }

            [Pure]
            public bool Equals(BaseUrlMessageSerialTuple other)
            {
                return this._messageSerial == other._messageSerial && StringComparer.OrdinalIgnoreCase.Equals(this._baseUrl, other._baseUrl);
            }

            [Pure]
            public override bool Equals(object obj)
            {
                if (obj is BaseUrlMessageSerialTuple)
                    return this.Equals((BaseUrlMessageSerialTuple)obj);

                return false;
            }

            [Pure]
            public override int GetHashCode()
            {
                return this._messageSerial.GetHashCode() ^ (this._baseUrl != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(this._baseUrl) : 0);
            }

            public override string ToString()
            {
                return string.Format("{0} {1}", this._baseUrl, this._messageSerial);
            }

            #endregion
        }

    }
}
