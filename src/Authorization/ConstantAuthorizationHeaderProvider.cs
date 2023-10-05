namespace DG.Common.Http.Authorization
{
    /// <summary>
    /// An implementation of <see cref="IAuthorizationHeaderProvider"/> that returns a constant value.
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of <see cref="ConstantAuthorizationHeaderProvider"/>.
        /// </summary>
        /// <param name="authorizationHeaderValue"></param>
        public ConstantAuthorizationHeaderProvider(string authorizationHeaderValue)
        {
            _authorizationHeaderValue = authorizationHeaderValue;
        }
    }
}
