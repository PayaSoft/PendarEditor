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


        public static async Task<JObject> GetMessageInfoAsync(this MessageSessionData context, CancellationTokenSource cancellationTokenSource = null)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            if (context.MessageData == null)
                await context.LoadMessageInfoAsync(cancellationTokenSource);
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

            var data = await Utility.HttpGetResponseAsync<JObject>(context.BaseUrl, $"/api/Messages/store/{context.StoreIndex}/info?uid={context.MessageId}", context.Token);


            context.MessageData = Utility.GetStandardResponseResult(data) as JObject;

            return context.MessageData;
        }


        public static async Task<byte[]> GetMessageContentAsync(this MessageSessionData context, bool insertHeader, bool insertSigns, bool insertSignImage, bool insertCopyText, bool insertRemarks, CancellationTokenSource cancellationTokenSource)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            var query = string.Format(@"uid={0}&insertHeader={1}&insertSigns={2}&insertSignImage={5}&insertCopyText={3}&insertRemarks={4}", context.MessageId, insertHeader, insertSigns, insertCopyText, insertRemarks, insertSignImage);
            var url = $"/api/Messages/store/{context.StoreIndex}/body/export/docx?{query}";

            var data = await Utility.HttpGetResponseAsync(context.BaseUrl, url, context.Token);

            return data;
        }


        public static async Task<ActionResult> SetBodyAsync(this MessageSessionData context, byte[] body, ExportFormat format = ExportFormat.Docx, string copyText = null, string pageSize = null, string pageOrientation = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            var pData = new Dictionary<string, string>
            {
                [@"body"] = body != null ? Convert.ToBase64String(body, Base64FormattingOptions.None) : null
            };

            var jData = JsonConvert.SerializeObject(pData).ToString();

            var content = new StringContent(jData, System.Text.Encoding.UTF8, "application/json");

            try
            {
                var data = await Utility.HttpPatchRequestAsync<JObject>(context.BaseUrl, $"/api/Messages/store/{context.StoreIndex}/body/docx?uid={context.MessageId}", content, context.Token, cancellationTokenSource);

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


        #endregion
    }
}