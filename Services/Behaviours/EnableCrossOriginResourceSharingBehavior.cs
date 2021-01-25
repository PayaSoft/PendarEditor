using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Paya.Automation.Editor.Services.Behaviours
{
	public class EnableCrossOriginResourceSharingBehavior : BehaviorExtensionElement, IEndpointBehavior
	{
		#region Public Properties

		public override Type BehaviorType
		{
			get { return typeof (EnableCrossOriginResourceSharingBehavior); }
		}

		#endregion

		#region Public Methods and Operators

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			var requiredHeaders = new ConcurrentDictionary<string, string>();

			requiredHeaders["Access-Control-Allow-Origin"] = "*";
			requiredHeaders["Access-Control-Request-Method"] = "POST,GET,PUT,DELETE,OPTIONS";
            requiredHeaders["Access-Control-Allow-Headers"] = "X-Requested-With,Content-Type,X-ConnectionId,X-Date,User-Agent,Referer,Origin,Accept,Authorization";

            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new EnableCrossOriginResourceSharingMessageInspector(requiredHeaders));
		}

		public void Validate(ServiceEndpoint endpoint)
		{
		}

		#endregion

		#region Methods

		protected override object CreateBehavior()
		{
			return new EnableCrossOriginResourceSharingBehavior();
		}

		#endregion
	}
}
