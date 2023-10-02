using DG.Common.Exceptions;
using System;
using System.Text;

namespace DG.Common.Http.Authorization
{
    public sealed class AuthorizationHeader
    {
        private readonly string _scheme;
        private readonly string _credentials;

        public string AuthenticationScheme => _scheme;
        public string Credentials => _credentials;
        public bool HasScheme => !string.IsNullOrEmpty(_scheme);

        public string GetHeaderValue()
        {
            if (!HasScheme)
            {
                return _credentials;
            }
            return _scheme + " " + _credentials;
        }

        public AuthorizationHeader(string scheme, string credentials)
        {
            _scheme = scheme;
            _credentials = credentials;
        }

        public static AuthorizationHeader With(string scheme, string credentials)
        {
            return new AuthorizationHeader(scheme, credentials);
        }

        /// <summary>
        /// Returns a new instance of <see cref="AuthorizationHeader"/> for an authorization header that has no authentication scheme.
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public static AuthorizationHeader WithoutScheme(string credentials)
        {
            return new AuthorizationHeader(null, credentials);
        }

        /// <summary>
        /// Returns a new instance of <see cref="AuthorizationHeader"/> for a basic authorization header that has username and password, seperated by a colon and base-64 encoded.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static AuthorizationHeader ForBasic(string username, string password)
        {
            ThrowIf.Parameter.IsNullOrEmpty(username, nameof(username));
            ThrowIf.Parameter.IsNullOrEmpty(password, nameof(password));
            ThrowIf.Parameter.Matches(username, u => u.Contains(":"), nameof(username), "username may not contain a colon.");
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));
            return new AuthorizationHeader("Basic", encoded);
        }

        /// <summary>
        /// Returns a new instance of <see cref="AuthorizationHeader"/> for a bearer authorization header that has an access token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static AuthorizationHeader ForBearer(string token)
        {
            return new AuthorizationHeader("Bearer", token);
        }

        /// <summary>
        /// Returns a new instance of <see cref="AuthorizationHeader"/> for a Netotiate/NTLM authorization header that has GSSAPI-data.
        /// </summary>
        /// <param name="gssapi"></param>
        /// <returns></returns>
        public static AuthorizationHeader ForNegotiate(string gssapi)
        {
            return new AuthorizationHeader("Negotiate", gssapi);
        }

        /// <summary>
        /// Returns a new instance of <see cref="AuthorizationHeader"/> for an Amazon AWS4-HMAC-SHA256 authorization header that has a <paramref name="credential"/>, <paramref name="signedHeaders"/> and a <paramref name="signature"/>.
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
        public static AuthorizationHeader ForAws(string credential, string[] signedHeaders, byte[] signature)
        {
            string headerList = string.Join(";", signedHeaders);
            string signatureHex = BitConverter.ToString(signature).Replace("-", "").ToLowerInvariant();

            string headerValue = $"Credential={credential},SignedHeaders={headerList},Signature={signatureHex}";
            return new AuthorizationHeader("AWS4-HMAC-SHA256", headerValue);
        }
    }
}
