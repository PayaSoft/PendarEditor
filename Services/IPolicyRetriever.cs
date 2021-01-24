namespace Paya.Automation.Editor.Services
{
	using System.IO;
	using System.ServiceModel;
	using System.ServiceModel.Web;

	/// <summary>
	///     Specifies the <see cref="IPolicyRetriever" /> interface used to enable corss domain WCF service in
	///     silverlight.
	/// </summary>
	[ServiceContract(Namespace = Internal.DefaultNamespace)]
	public interface IPolicyRetriever
	{
		#region Public Methods and Operators

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		[OperationContract]
		[WebGet(UriTemplate = "/favicon.ico")]
		Stream GetFavicon();

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		[OperationContract]
		[WebGet(UriTemplate = "/crossdomain.xml", ResponseFormat = WebMessageFormat.Xml)]
		Stream GetFlashPolicy();

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		[OperationContract]
		[WebGet(UriTemplate = "/clientaccesspolicy.xml", ResponseFormat = WebMessageFormat.Xml)]
		Stream GetSilverlightPolicy();

		#endregion
	}
}