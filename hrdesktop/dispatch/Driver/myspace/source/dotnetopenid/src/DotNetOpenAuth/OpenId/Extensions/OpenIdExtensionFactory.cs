﻿//-----------------------------------------------------------------------
// <copyright file="OpenIdExtensionFactory.cs" company="Andrew Arnott">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DotNetOpenAuth.OpenId.Extensions {
	using System.Collections.Generic;
	using DotNetOpenAuth.Messaging;
	using DotNetOpenAuth.OpenId.ChannelElements;
	using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
	using DotNetOpenAuth.OpenId.Extensions.ProviderAuthenticationPolicy;
	using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
	using DotNetOpenAuth.OpenId.Messages;
    using DotNetOpenAuth.OpenId.Extensions.OAuth;

	/// <summary>
	/// An OpenID extension factory that supports registration so that third-party
	/// extensions can add themselves to this library's supported extension list.
	/// </summary>
	internal class OpenIdExtensionFactory : IOpenIdExtensionFactory {
		/// <summary>
		/// A collection of the registered OpenID extensions.
		/// </summary>
		private List<CreateDelegate> registeredExtensions = new List<CreateDelegate>();

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIdExtensionFactory"/> class.
		/// </summary>
		internal OpenIdExtensionFactory() {
			this.RegisterExtension(ClaimsRequest.Factory);
			this.RegisterExtension(ClaimsResponse.Factory);
			this.RegisterExtension(FetchRequest.Factory);
			this.RegisterExtension(FetchResponse.Factory);
			this.RegisterExtension(StoreRequest.Factory);
			this.RegisterExtension(StoreResponse.Factory);
			this.RegisterExtension(PolicyRequest.Factory);
			this.RegisterExtension(PolicyResponse.Factory);
            this.RegisterExtension(OAuthRequest.Factory);
            this.RegisterExtension(OAuthResponse.Factory);
		}

		/// <summary>
		/// A delegate that individual extensions may register with this factory.
		/// </summary>
		/// <param name="typeUri">The type URI of the extension.</param>
		/// <param name="data">The parameters associated specifically with this extension.</param>
		/// <param name="baseMessage">The OpenID message carrying this extension.</param>
		/// <returns>
		/// An instance of <see cref="IOpenIdMessageExtension"/> if the factory recognizes
		/// the extension described in the input parameters; <c>null</c> otherwise.
		/// </returns>
		internal delegate IOpenIdMessageExtension CreateDelegate(string typeUri, IDictionary<string, string> data, IProtocolMessageWithExtensions baseMessage);

		#region IOpenIdExtensionFactory Members

		/// <summary>
		/// Creates a new instance of some extension based on the received extension parameters.
		/// </summary>
		/// <param name="typeUri">The type URI of the extension.</param>
		/// <param name="data">The parameters associated specifically with this extension.</param>
		/// <param name="baseMessage">The OpenID message carrying this extension.</param>
		/// <returns>
		/// An instance of <see cref="IOpenIdMessageExtension"/> if the factory recognizes
		/// the extension described in the input parameters; <c>null</c> otherwise.
		/// </returns>
		/// <remarks>
		/// This factory method need only initialize properties in the instantiated extension object
		/// that are not bound using <see cref="MessagePartAttribute"/>.
		/// </remarks>
		public IOpenIdMessageExtension Create(string typeUri, IDictionary<string, string> data, IProtocolMessageWithExtensions baseMessage) {
			foreach (var factoryMethod in this.registeredExtensions) {
				IOpenIdMessageExtension result = factoryMethod(typeUri, data, baseMessage);
				if (result != null) {
					return result;
				}
			}

			return null;
		}

		#endregion

		/// <summary>
		/// Registers a new extension delegate.
		/// </summary>
		/// <param name="creator">The factory method that can create the extension.</param>
		internal void RegisterExtension(CreateDelegate creator) {
			this.registeredExtensions.Add(creator);
		}
	}
}
