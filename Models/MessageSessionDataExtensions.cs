using System;
using Newtonsoft.Json;
using Paya.Automation.Models;

namespace Paya.Automation.Editor.Models
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Newtonsoft.Json.Linq;
    using NLog;


    public static class MessageSessionDataExtensions
    {
        #region Static Fields

        private static readonly Logger _Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Methods and Operators

        //public static void ConvertPdfAndDownloadDocument(this MessageSessionData context, string xamlBody)
        //{
        //}

        public static async Task<string> GetClassCodeAsync(this MessageSessionData context, CancellationTokenSource cancellationTokenSource = null)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            var d = await context.GetMessageDataAsync(cancellationTokenSource);
            if (d == null)
                return null;
            var c = d["Class"];
            if (c == null)
                return null;
            var code = c["Code"];
            return code == null ? null : code.Value<string>();
        }

        public static async Task<JObject> GetMessageDataAsync(this MessageSessionData context, CancellationTokenSource cancellationTokenSource = null)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            if (context.MessageData == null)
                await context.LoadMessageDataAsync(cancellationTokenSource);
            return context.MessageData;
        }

        public static async Task<JObject> GetMessageInfoAsync(this MessageSessionData context, CancellationTokenSource cancellationTokenSource = null)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            if (context.MessageData == null)
                await context.LoadMessageInfoAsync(cancellationTokenSource);
            return context.MessageData;
        }

        public static async Task<bool> HasPermissionAsync(this MessageSessionData context, [NotNull] string permissionName, CancellationTokenSource cancellationTokenSource = null)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            var messageData = await context.GetMessageDataAsync(cancellationTokenSource);
            if (messageData == null)
                return false;
            var permissions = messageData["Permissions"] as JObject;
            if (permissions == null)
                return false;
            var permision = permissions[permissionName];
            return permision != null && permision.Value<bool>();
        }

        public static async Task<bool> HasRoleAsync(this MessageSessionData context, [NotNull] string roleName)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            var roles = await context.GetRolesAsync();
            return roles != null && roles.Contains(roleName);
        }

        public static async Task<JObject> LoadMessageDataAsync(this MessageSessionData context, CancellationTokenSource cancellationTokenSource = null)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            var pdata = new Dictionary<string, string>();

            pdata["storeIndex"] = Convert.ToString(context.StoreIndex);
            pdata["messageId"] = Convert.ToString(context.MessageId);

            var data = await Utility.HttpPostRequestAsync<JObject>(context.BaseUrl, "/Message/Details", new FormUrlEncodedContent(pdata), context.Cookies, cancellationTokenSource);

            context.MessageData = Utility.GetStandardResponseResult(data) as JObject;

            return context.MessageData;
        }

        public static async Task<JObject> LoadMessageInfoAsync(this MessageSessionData context, CancellationTokenSource cancellationTokenSource = null)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            var pdata = new Dictionary<string, string>();

            pdata["storeIndex"] = Convert.ToString(context.StoreIndex);
            pdata["messageId"] = Convert.ToString(context.MessageId);

            var data = await Utility.HttpPostRequestAsync<JObject>(context.BaseUrl, "/Message/Info", new FormUrlEncodedContent(pdata), context.Cookies, cancellationTokenSource);

            context.MessageData = Utility.GetStandardResponseResult(data) as JObject;

            return context.MessageData;
        }

        public static async Task<byte[]> GetMessageContentAsync(this MessageSessionData context, bool insertHeader, bool insertSigns, bool insertSignImage, bool insertCopyText, bool insertRemarks, CancellationTokenSource cancellationTokenSource)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            var uri = new UriBuilder(new Uri(context.BaseUrl));
            uri.Path = @"Message/Content";
            uri.Query = string.Format(@"storeIndex={0}&messageId={1}&insertHeader={2}&insertSigns={3}&insertSignImage={6}&insertCopyText={4}&insertRemarks={5}", context.StoreIndex, context.MessageId, insertHeader, insertSigns, insertCopyText, insertRemarks, insertSignImage);

            var data = await Utility.HttpGetResponseAsync(uri.Uri, context.Cookies, cancellationTokenSource);

            return data;
        }

        public static async Task<byte[]> LoadMessageDataAsByte(this MessageSessionData context, string xamlBody)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            var pdata = new Dictionary<string, string>();
            pdata[@"storeIndex"] = Convert.ToString(context.StoreIndex);
            pdata[@"messageId"] = Convert.ToString(context.MessageId);
            pdata[@"format"] = Convert.ToString(ExportFormat.Jpeg);
            pdata[@"xamlBody"] = ExtendedFormUrlEncodedContent.EscapeDataString(xamlBody);
            var data = await Utility.HttpPostRequestBinaryAsync(context.BaseUrl, @"/Message/Export", new ExtendedFormUrlEncodedContent(pdata), context.Cookies);

            return data;
        }

        public static async Task GetUserDataAsync(this MessageSessionData context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            var pdata = new Dictionary<string, string>();

            pdata[@"storeIndex"] = Convert.ToString(context.StoreIndex);

            var data = await Utility.HttpPostRequestAsync<JObject>(context.BaseUrl, @"/Account/CurrentUser", new FormUrlEncodedContent(pdata), context.Cookies);

            var obj = Utility.GetStandardResponseResult(data) as JObject;

            context.UserId = obj[@"ID"].Value<long>();
            context.UserName = obj[@"UserName"].Value<string>();
        }

        public static async Task<JToken> PostRequestAsync(this MessageSessionData context, string url, object data, bool isStandardResult = true, CancellationTokenSource cancellationTokenSource = null)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            return await context.PostRequestAsync(url, new ExtendedFormUrlEncodedContent(data), isStandardResult, cancellationTokenSource);
        }

        public static async Task<JToken> PostRequestAsync(this MessageSessionData context, string url, HttpContent content, bool isStandardResult = true, CancellationTokenSource cancellationTokenSource = null)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            var response = await Utility.HttpPostRequestAsync<JToken>(context.BaseUrl, url, content, context.Cookies, cancellationTokenSource);

            if (response == null)
                return null;

            return isStandardResult ? Utility.GetStandardResponseResult(response) : Utility.GetResponseResult(response);
        }

        public static async Task<JToken> PostRequestAsync(this MessageSessionData context, string url, IEnumerable<KeyValuePair<string, string>> data, bool isStandardResult = true, CancellationTokenSource cancellationTokenSource = null)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            return await context.PostRequestAsync(url, new ExtendedFormUrlEncodedContent(data), isStandardResult, cancellationTokenSource);
        }

        public static async Task<ActionResult> SaveBodyAsync(this MessageSessionData context, string body, string xamlBody, string copyText, string pageSize, string pageOrientation, CancellationTokenSource cancellationTokenSource = null)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            var pdata = new Dictionary<string, string>();

            pdata[@"storeIndex"] = Convert.ToString(context.StoreIndex);
            pdata[@"messageId"] = Convert.ToString(context.MessageId);
            pdata[@"body"] = ExtendedFormUrlEncodedContent.EscapeDataString(body);
            pdata[@"xamlbody"] = ExtendedFormUrlEncodedContent.EscapeDataString(xamlBody);
            pdata[@"CopyText"] = copyText;
            pdata[@"PageSize"] = pageSize;
            pdata[@"PageOrientation"] = pageOrientation;

            try
            {
                var data = await Utility.HttpPostRequestAsync<JObject>(context.BaseUrl, "/Message/SaveBody", new ExtendedFormUrlEncodedContent(pdata), context.Cookies, cancellationTokenSource);

                if (data == null || data["Status"] == null)
                    return new ActionResult(false, null);

                if (data["Status"]["Succeed"].Value<bool>())
                    return ActionResult.Success;

                string message = data["Status"]["Message"].Value<string>();
                return new ActionResult(false, message);
            }
            catch (Exception exp)
            {
                if (_Logger.IsErrorEnabled)
                    _Logger.Error(exp, "Error while getting message data");
                return new ActionResult(false, exp.Message);
            }
        }

        public static async Task<ActionResult> SetBodyAsync(this MessageSessionData context, byte[] body, ExportFormat format = ExportFormat.Docx, string copyText = null, string pageSize = null, string pageOrientation = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            var pdata = new Dictionary<string, string>();

            pdata[@"storeIndex"] = Convert.ToString(context.StoreIndex, CultureInfo.InvariantCulture);
            pdata[@"messageId"] = Convert.ToString(context.MessageId, CultureInfo.InvariantCulture);

            if (!string.IsNullOrWhiteSpace(copyText))
                pdata[@"CopyText"] = copyText;

            if (!string.IsNullOrWhiteSpace(pageSize))
                pdata[@"PageSize"] = pageSize;

            if (!string.IsNullOrWhiteSpace(pageOrientation))
                pdata[@"PageOrientation"] = pageOrientation;

            pdata[@"bodyFormat"] = Convert.ToString(format, CultureInfo.InvariantCulture);
            pdata[@"body"] = body != null ? Convert.ToBase64String(body, Base64FormattingOptions.None) : null;

            try
            {
                var data = await Utility.HttpPostRequestAsync<JObject>(context.BaseUrl, @"/Message/SetBody", new ExtendedFormUrlEncodedContent(pdata), context.Cookies, cancellationTokenSource);

                if (data == null || data["Status"] == null)
                    return new ActionResult(false, null);

                if (data["Status"]["Succeed"].Value<bool>())
                    return ActionResult.Success;

                string message = data["Status"]["Message"].Value<string>();
                return new ActionResult(false, message);
            }
            catch (Exception exp)
            {
                if (_Logger.IsErrorEnabled)
                    _Logger.Error(exp, "Error while getting message data");
                return new ActionResult(false, exp.Message);
            }
        }

        public static async Task SendEmail(this MessageSessionData context, string xamlBodyBase64String, IEnumerable<string> receipts, IEnumerable<string> ccReceipts, IEnumerable<string> bccReceipts, IEnumerable<int> attachmentIds, bool sendBody, EceData ece)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            var pdata = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("storeIndex", Convert.ToString(context.StoreIndex)),
                new KeyValuePair<string, string>("messageId", Convert.ToString(context.MessageId)),
                new KeyValuePair<string, string>("xamlBody", Convert.ToString(xamlBodyBase64String)),
                new KeyValuePair<string, string>("SendLetter", Convert.ToString(sendBody))
            };

            if (receipts != null)
                pdata.AddRange(receipts.Select(r => new KeyValuePair<string, string>("receipts[]", r)));
            if (ccReceipts != null)
                pdata.AddRange(ccReceipts.Select(r => new KeyValuePair<string, string>("ccReceipts[]", r)));
            if (bccReceipts != null)
                pdata.AddRange(bccReceipts.Select(r => new KeyValuePair<string, string>("bccReceipts[]", r)));
            if (attachmentIds != null)
                pdata.AddRange(attachmentIds.Select(r => new KeyValuePair<string, string>("attachmentIds[]", r.ToString())));

            if (ece != null)
            {
                pdata.AddRange(Utility.GetUrlEncodedData(new { ece }));
            }

            var result = await Utility.HttpPostRequestAsync<JObject>(context.BaseUrl, "/Email/Send", new ExtendedFormUrlEncodedContent(pdata), context.Cookies);

            if (result == null || result["Status"] == null)
                throw new InvalidOperationException("Invalid response");
            if (!result["Status"]["Succeed"].Value<bool>())
                throw new InvalidOperationException(result["Status"]["Message"].Value<string>());
            if (!result["Data"].Value<bool>())
                throw new InvalidOperationException("Sending email failed.");
        }

        #endregion

        #region Methods

        private static async Task<ISet<string>> GetRolesAsync(this MessageSessionData context)
        {
            return context.Roles ?? (context.Roles = await context.LoadRolesAsync());
        }

        private static async Task<ISet<string>> LoadRolesAsync(this MessageSessionData context)
        {
            var pdata = new Dictionary<string, string>();

            pdata["storeIndex"] = Convert.ToString(context.StoreIndex);

            try
            {
                var data = await Utility.HttpPostRequestAsync<JObject>(context.BaseUrl, "/User/Roles", new FormUrlEncodedContent(pdata), context.Cookies);

                if (data != null && data["Status"] != null)
                {
                    if (data["Status"]["Succeed"].Value<bool>())
                    {
                        var roles = data["Data"] as JArray;
                        context.Roles = roles != null ? new HashSet<string>(roles.Select(x => x["Code"].Value<string>()), StringComparer.OrdinalIgnoreCase) : null;
                    }
                    else
                    {
                        context.Roles = null;

                        if (_Logger.IsErrorEnabled)
                        {
                            _Logger.Error(data["Status"]["Message"].Value<string>());
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                context.Roles = null;

                if (_Logger.IsErrorEnabled)
                {
                    _Logger.Error(exp, "Error while getting message data");
                }
            }

            return context.Roles;
        }

        #endregion
    }
}