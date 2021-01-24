using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Paya.Automation.Editor.Services.Behaviours
{
    using System.Net;
    using System.Security;

    public class EnableCrossOriginResourceSharingMessageInspector : IDispatchMessageInspector
	{
		#region Fields

		private readonly IDictionary<string, string> _requiredHeaders;


        private static readonly NLog.Logger _Logger = NLog.LogManager.GetCurrentClassLogger();

		#endregion

		#region Constructors and Destructors

		public EnableCrossOriginResourceSharingMessageInspector(IDictionary<string, string> headers)
		{
			this._requiredHeaders = headers ?? new ConcurrentDictionary<string, string>();
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Called after an inbound message has been received but before the message is dispatched to the intended operation.
		/// </summary>
		/// <param name="request">The request message.</param>
		/// <param name="channel">The incoming channel.</param>
		/// <param name="instanceContext">The current service instance.</param>
		/// <returns>
		/// The object used to correlate state. This object is passed back in the <see cref="M:System.ServiceModel.Dispatcher.IDispatchMessageInspector.BeforeSendReply(System.ServiceModel.Channels.Message@,System.Object)" /> method.
		/// </returns>
		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
		    if (request.Properties.ContainsKey("System.ServiceModel.Channels.RemoteEndpointMessageProperty"))
		    {
		        var req = request.Properties["System.ServiceModel.Channels.RemoteEndpointMessageProperty"] as RemoteEndpointMessageProperty;
		        if (req != null)
		        {
		            IPAddress adr;
		            if (IPAddress.TryParse(req.Address, out adr))
		            {
		                if (!adr.Equals(IPAddress.IPv6Loopback) && !adr.Equals(IPAddress.Loopback))
		                {
		                    if (_Logger.IsErrorEnabled)
		                        _Logger.Error("Invalid request from {0}", adr);

		                    throw new FaultException<SecurityException>(new SecurityException(string.Format("The IP address '{0}' is not valid.", adr)), new FaultReason("Invalid Request"), new FaultCode("INVALID_REQUEST"));
		                }
		            }
		        }
		    }

		    return null;
		}

		public void BeforeSendReply(ref Message reply, object correlationState)
		{
		    if (!reply.Properties.ContainsKey("httpResponse"))
		        return;
            var httpHeader = (HttpResponseMessageProperty) reply.Properties["httpResponse"];
			foreach (var item in this._requiredHeaders)
			{
				httpHeader.Headers.Add(item.Key, item.Value);
			}
		}

		#endregion
	}
}