using System;

namespace DG.Common.Http.Authorization
{
    public class ExpiringAuthorization
    {
        private readonly AuthorizationHeader _authorizationHeader;
        private readonly DateTimeOffset _expires;

        public AuthorizationHeader Header => _authorizationHeader;
        public virtual DateTimeOffset ExpirationDate => _expires;
        public bool IsExpired => DateTimeOffset.UtcNow > ExpirationDate;

        public string GetHeaderValue()
        {
            return _authorizationHeader.GetHeaderValue();
        }

        public ExpiringAuthorization(AuthorizationHeader authorizationHeader, DateTimeOffset expires)
        {
            _authorizationHeader = authorizationHeader;
            _expires = expires;
        }

        public static ExpiringAuthorization With(AuthorizationHeader authorizationHeader, DateTimeOffset expires)
        {
            return new ExpiringAuthorization(authorizationHeader, expires);
        }
    }
}
