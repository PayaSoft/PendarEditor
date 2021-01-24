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
        [WebInvoke(UriTemplate = "/OpenBody", Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        void OpenBody(string baseUrl, int storeIndex, string folderId, int messageId, int messageSerial, bool insertHeader, bool insertSigns, bool insertSignImage, bool insertCopyText, bool insertRemarks, string cipher, Dictionary<string, string> encryptedCookies);

        [OperationContract(Name = "Print")]
        [WebInvoke(UriTemplate = "/Print", Method = "*", BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        void Print(string baseUrl, int storeIndex, string folderId, int messageId, int messageSerial, bool insertHeader, bool insertSigns, bool insertSignImage, bool insertCopyText, bool insertRemarks, bool withPreview, string cipher, Dictionary<string, string> encryptedCookies);

        #endregion
    }
}
