﻿//-----------------------------------------------------------------------
// <copyright file="TimestampEncoder.cs" company="Andrew Arnott">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DotNetOpenAuth.OAuthWrap.ChannelElements {
	using System;
	using System.Globalization;
	using DotNetOpenAuth.Messaging.Reflection;

	internal class TimestampEncoder : IMessagePartEncoder {
		/// <summary>
		/// The reference date and time for calculating time stamps.
		/// </summary>
		private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		/// <summary>
		/// Initializes a new instance of the <see cref="TimestampEncoder"/> class.
		/// </summary>
		internal TimestampEncoder() {
		}

		/// <summary>
		/// Encodes the specified value.
		/// </summary>
		/// <param name="value">The value.  Guaranteed to never be null.</param>
		/// <returns>
		/// The <paramref name="value"/> in string form, ready for message transport.
		/// </returns>
		public string Encode(object value) {
			var timestamp = (DateTime) value;
			TimeSpan secondsSinceEpoch = timestamp - Epoch;
			return secondsSinceEpoch.TotalSeconds.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Decodes the specified value.
		/// </summary>
		/// <param name="value">The string value carried by the transport.  Guaranteed to never be null, although it may be empty.</param>
		/// <returns>
		/// The deserialized form of the given string.
		/// </returns>
		/// <exception cref="FormatException">Thrown when the string value given cannot be decoded into the required object type.</exception>
		public object Decode(string value) {
			var secondsSinceEpoch = Convert.ToInt32(value, CultureInfo.InvariantCulture);
			return Epoch.AddSeconds(secondsSinceEpoch);
		}
	}
}
