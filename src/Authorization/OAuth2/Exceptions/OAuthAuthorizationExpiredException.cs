using System;
using System.Runtime.Serialization;

namespace DG.Common.Http.Authorization.OAuth2.Exceptions
{
    /// <summary>
    /// Represents errors that occur when OAuth authorization has been expired, and could not be refreshed.
    /// </summary>
    [Serializable]
    public sealed class OAuthAuthorizationExpiredException : Exception
    {
        private readonly string _state;

        /// <summary>
        /// The state property of the request that could not be found.
        /// </summary>
        public string State => _state;

        /// <inheritdoc/>
        public override string Message => $"Authorization for request with state {_state} has expired, and could not be refreshed.";

        /// <summary>
        /// Initializes a new instance of <see cref="OAuthAuthorizationExpiredException"/> with the given <paramref name="state"/>.
        /// </summary>
        /// <param name="state"></param>
        public OAuthAuthorizationExpiredException(string state)
        {
            _state = state;
        }

        private OAuthAuthorizationExpiredException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _state = info.GetString(nameof(State));
        }

        /// <summary>
        /// Immediately throws a <see cref="OAuthAuthorizationExpiredException"/> with the given state as <see cref="State"/>.
        /// </summary>
        /// <param name="state"></param>
        /// <exception cref="OAuthAuthorizationExpiredException"></exception>
        internal static void ThrowForState(string state)
        {
            throw new OAuthAuthorizationExpiredException(state);
        }
    }
}
