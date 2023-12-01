using DG.Common.Exceptions;
using DG.Common.Http.Authorization;
using System;
using System.Text;

namespace DG.Common.Http.Fluent
{
    /// <summary>
    /// Represents the value for an <c>Authorization</c> header.
    /// </summary>
    public sealed class FluentAuthorization : IAuthorizationHeaderProvider
    {
        private readonly string _scheme;
        private readonly string _credentials;

        /// <summary>
        /// <para>The Authentication scheme that defines how the credentials are encoded.</para>
        /// <para>Note this is optional.</para>
        /// </summary>
        public string AuthenticationScheme => _scheme;

        /// <summary>
        /// The credentials, encoded according to the specified scheme.
        /// </summary>
        public string Credentials => _credentials;

        /// <summary>
        /// Indicates if an <see cref="AuthenticationScheme"/> is specified.
        /// </summary>
        public bool HasScheme => !string.IsNullOrEmpty(_scheme);

        /// <summary>
        /// Returns a string that can be used as the value of an HTTP <c>Authorization</c> header.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (!HasScheme)
            {
                return _credentials;
            }
            return _scheme + " " + _credentials;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FluentAuthorization"/> with the given scheme and credentials.
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="credentials"></param>
        public FluentAuthorization(string scheme, string credentials)
        {
            _scheme = scheme;
            _credentials = credentials;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FluentAuthorization"/> with the given scheme and credentials.
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public static FluentAuthorization With(string scheme, string credentials)
        {
            return new FluentAuthorization(scheme, credentials);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FluentAuthorization"/> for an authorization header that has no authentication scheme.
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public static FluentAuthorization WithoutScheme(string credentials)
        {
            return new FluentAuthorization(null, credentials);
        }

        /// <summary>
        /// Returns a new instance of <see cref="FluentAuthorization"/> for a basic authorization header that has username and password, seperated by a colon and base-64 encoded.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static FluentAuthorization ForBasic(string username, string password)
        {
            ThrowIf.Parameter.IsNullOrEmpty(username, nameof(username));
            ThrowIf.Parameter.IsNullOrEmpty(password, nameof(password));
            ThrowIf.Parameter.Matches(username, u => u.Contains(":"), nameof(username), "username may not contain a colon.");
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));
            return new FluentAuthorization("Basic", encoded);
        }

        /// <summary>
        /// Returns a new instance of <see cref="FluentAuthorization"/> for a bearer authorization header that has an <paramref name="accessToken"/>.
        /// </summary>
        /// <param name="accessToken">An OAuth 2.0 bearer access token</param>
        /// <returns></returns>
        public static FluentAuthorization ForBearer(string accessToken)
        {
            return new FluentAuthorization("Bearer", accessToken);
        }

        /// <summary>
        /// Returns a new instance of <see cref="FluentAuthorization"/> for a Netotiate/NTLM authorization header that has <paramref name="GssApiData"/>.
        /// </summary>
        /// <param name="GssApiData">GSSAPI data</param>
        /// <returns></returns>
        public static FluentAuthorization ForNegotiate(string GssApiData)
        {
            return new FluentAuthorization("Negotiate", GssApiData);
        }

        /// <summary>
        /// Returns a new instance of <see cref="FluentAuthorization"/> for an Amazon AWS4-HMAC-SHA256 authorization header that has a <paramref name="credential"/>, <paramref name="signedHeaders"/> and a <paramref name="signature"/>.
        /// </summary>
        /// <param name="credential">
        ///     <para>Your access key ID and the scope information, which includes the date, region, and service that were used to calculate the signature.</para>
        ///     <para>This mus be in the format <c><![CDATA[<your-access-key-id>/<date>/<aws-region>/<aws-service>/aws4_request]]></c>.</para>
        /// </param>
        /// <param name="signedHeaders">
        ///     <para>A list of request headers that you used to compute the signature. The list includes header names only, and the header names must be in lowercase. </para>
        ///     <para>For example: <c><![CDATA[host;range;x-amz-date]]></c>.</para>
        /// </param>
        /// <param name="signature">The 256-bit signature.</param>
        /// <returns></returns>
        public static FluentAuthorization ForAws(string credential, string[] signedHeaders, byte[] signature)
        {
            string headerList = string.Join(";", signedHeaders);
            string signatureHex = BitConverter.ToString(signature).Replace("-", "").ToLowerInvariant();

            string headerValue = $"Credential={credential},SignedHeaders={headerList},Signature={signatureHex}";
            return new FluentAuthorization("AWS4-HMAC-SHA256", headerValue);
        }

        /// <inheritdoc/>
        bool IAuthorizationHeaderProvider.IsAuthorized => true;

        /// <inheritdoc/>
        string IAuthorizationHeaderProvider.GetAuthorizationHeaderValue()
        {
            return ToString();
        }
    }
}
