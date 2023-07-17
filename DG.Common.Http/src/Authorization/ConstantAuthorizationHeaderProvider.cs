namespace DG.Common.Http.Authorization
{
    public class ConstantAuthorizationHeaderProvider : IAuthorizationHeaderProvider
    {
        private readonly string _authorizationHeaderValue;

        /// <inheritdoc/>
        public bool IsAuthorized => !string.IsNullOrEmpty(_authorizationHeaderValue);

        /// <inheritdoc/>
        public string GetAuthorizationHeaderValue()
        {
            return _authorizationHeaderValue;
        }

        public ConstantAuthorizationHeaderProvider(string authorizationHeaderValue)
        {
            _authorizationHeaderValue = authorizationHeaderValue;
        }
    }
}
