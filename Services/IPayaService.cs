namespace Paya.Automation.Editor.Services
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Web.Script.Services;

    [ServiceContract(Namespace = Internal.DefaultNamespace, SessionMode = SessionMode.NotAllowed)]
    public interface IPayaService
    {
        #region Public Methods and Operators

        [OperationContract(Name = "Ping")]
        [WebInvoke(UriTemplate = "/Ping", Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        void Ping();

        [OperationContract(Name = "OpenBody")]
        [WebInvoke(UriTemplate = "/OpenBody", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        void OpenBody(string baseUrl, int storeIndex, string messageId, int messageSerial, bool insertHeader, bool insertSigns, bool insertSignImage, bool insertCopyText, bool insertRemarks, string token, string updateUrl);

        [OperationContract]
        [WebInvoke(UriTemplate = "/OpenBody", Method = "OPTIONS")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        void OpenBodyOptions();


        [OperationContract]
        [WebInvoke(UriTemplate = "/Print", Method = "OPTIONS")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        void PrintOptions();

        [OperationContract(Name = "Print")]
        [WebInvoke(UriTemplate = "/Print", Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        void Print(string baseUrl, int storeIndex, string messageId, int messageSerial, bool insertHeader, bool insertSigns, bool insertSignImage, bool insertCopyText, bool insertRemarks, bool withPreview, string token);

        #endregion
    }
}
