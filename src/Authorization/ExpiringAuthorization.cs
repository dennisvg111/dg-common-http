using System;

namespace DG.Common.Http.Authorization
{
    public class ExpiringAuthorization
    {
        private readonly AuthorizationHeaderValue _authorizationHeader;
        private readonly DateTimeOffset _expires;

        public AuthorizationHeaderValue Header => _authorizationHeader;
        public virtual DateTimeOffset ExpirationDate => _expires;
        public bool IsExpired => DateTimeOffset.UtcNow > ExpirationDate;

        public string GetHeaderValue()
        {
            return _authorizationHeader.ToString();
        }

        public ExpiringAuthorization(AuthorizationHeaderValue authorizationHeader, DateTimeOffset expires)
        {
            _authorizationHeader = authorizationHeader;
            _expires = expires;
        }

        public static ExpiringAuthorization With(AuthorizationHeaderValue authorizationHeader, DateTimeOffset expires)
        {
            return new ExpiringAuthorization(authorizationHeader, expires);
        }
    }
}
