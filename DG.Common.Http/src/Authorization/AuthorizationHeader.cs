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
    }
}
