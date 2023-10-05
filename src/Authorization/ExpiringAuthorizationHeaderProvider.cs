using DG.Common.Http.Exceptions;

namespace DG.Common.Http.Authorization
{
    public class ExpiringAuthorizationHeaderProvider : IAuthorizationHeaderProvider
    {
        private readonly IExpiringAuthorizationProvider _authorizationRefreshLogic;

        private ExpiringAuthorization _cachedAuthorizationHeader;

        public bool CacheExpired => _cachedAuthorizationHeader == null || _cachedAuthorizationHeader.IsExpired;

        /// <inheritdoc/>
        public bool IsAuthorized
        {
            get
            {
                if (!CacheExpired)
                {
                    return true;
                }
                return TryRefreshAuthorization();
            }
        }

        public ExpiringAuthorizationHeaderProvider(IExpiringAuthorizationProvider authorizationRefreshLogic)
        {
            _authorizationRefreshLogic = authorizationRefreshLogic;
        }

        private bool TryRefreshAuthorization()
        {
            if (!_authorizationRefreshLogic.TryRefreshAuthorization(out ExpiringAuthorization authorization))
            {
                return false;
            }
            _cachedAuthorizationHeader = authorization;
            return true;
        }

        /// <inheritdoc/>
        public string GetAuthorizationHeaderValue()
        {
            if (!CacheExpired)
            {
                return _cachedAuthorizationHeader.GetHeaderValue();
            }
            if (!TryRefreshAuthorization())
            {
                throw HttpAuthorizationException.DuringRefresh(_cachedAuthorizationHeader == null);
            }
            return _cachedAuthorizationHeader.GetHeaderValue();
        }
    }
}
