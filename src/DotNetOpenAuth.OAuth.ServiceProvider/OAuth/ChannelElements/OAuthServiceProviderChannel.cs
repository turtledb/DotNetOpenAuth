﻿//-----------------------------------------------------------------------
// <copyright file="OAuthServiceProviderChannel.cs" company="Andrew Arnott">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DotNetOpenAuth.OAuth.ChannelElements {
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Text;
	using DotNetOpenAuth.Messaging;
	using DotNetOpenAuth.Messaging.Bindings;

	/// <summary>
	/// The messaging channel for OAuth 1.0(a) Service Providers.
	/// </summary>
	internal class OAuthServiceProviderChannel : OAuthChannel {
		/// <summary>
		/// Initializes a new instance of the <see cref="OAuthServiceProviderChannel"/> class.
		/// </summary>
		/// <param name="signingBindingElement">The binding element to use for signing.</param>
		/// <param name="store">The web application store to use for nonces.</param>
		/// <param name="tokenManager">The token manager instance to use.</param>
		/// <param name="securitySettings">The security settings.</param>
		/// <param name="messageTypeProvider">The message type provider.</param>
		[SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Diagnostics.Contracts.__ContractsRuntime.Requires<System.ArgumentNullException>(System.Boolean,System.String,System.String)", Justification = "Code contracts"), SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "securitySettings", Justification = "Code contracts")]
		internal OAuthServiceProviderChannel(ITamperProtectionChannelBindingElement signingBindingElement, INonceStore store, IServiceProviderTokenManager tokenManager, ServiceProviderSecuritySettings securitySettings, IMessageFactory messageTypeProvider = null)
			: base(
			signingBindingElement,
			store,
			tokenManager,
			securitySettings,
			messageTypeProvider ?? new OAuthServiceProviderMessageFactory(tokenManager),
			InitializeBindingElements(signingBindingElement, store, tokenManager, securitySettings)) {
			Contract.Requires<ArgumentNullException>(tokenManager != null);
			Contract.Requires<ArgumentNullException>(securitySettings != null);
			Contract.Requires<ArgumentNullException>(signingBindingElement != null);
			Contract.Requires<ArgumentException>(signingBindingElement.SignatureCallback == null, OAuthStrings.SigningElementAlreadyAssociatedWithChannel);
		}

		/// <summary>
		/// Gets the consumer secret for a given consumer key.
		/// </summary>
		/// <param name="consumerKey">The consumer key.</param>
		/// <returns>The consumer secret.</returns>
		protected override string GetConsumerSecret(string consumerKey) {
			return ((IServiceProviderTokenManager)this.TokenManager).GetConsumer(consumerKey).Secret;
		}

		/// <summary>
		/// Initializes the binding elements for the OAuth channel.
		/// </summary>
		/// <param name="signingBindingElement">The signing binding element.</param>
		/// <param name="store">The nonce store.</param>
		/// <param name="tokenManager">The token manager.</param>
		/// <param name="securitySettings">The security settings.</param>
		/// <returns>
		/// An array of binding elements used to initialize the channel.
		/// </returns>
		private static new IChannelBindingElement[] InitializeBindingElements(ITamperProtectionChannelBindingElement signingBindingElement, INonceStore store, ITokenManager tokenManager, SecuritySettings securitySettings) {
			Contract.Requires(securitySettings != null);

			var bindingElements = OAuthChannel.InitializeBindingElements(signingBindingElement, store, tokenManager, securitySettings);

			var spTokenManager = tokenManager as IServiceProviderTokenManager;
			var serviceProviderSecuritySettings = securitySettings as ServiceProviderSecuritySettings;
			bindingElements.Insert(0, new TokenHandlingBindingElement(spTokenManager, serviceProviderSecuritySettings));

			return bindingElements.ToArray();
		}
	}
}
