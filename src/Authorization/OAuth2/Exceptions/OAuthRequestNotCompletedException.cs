using DG.Common.Http.Authorization.OAuth2.Data;
using System;
using System.Runtime.Serialization;

namespace DG.Common.Http.Authorization.OAuth2.Exceptions
{
    /// <summary>
    /// Represents errors that occur when an <see cref="OAuthRequest"/> has not been completed.
    /// </summary>
    [Serializable]
    public sealed class OAuthRequestNotCompletedException : Exception
    {
        private readonly string _state;

        /// <summary>
        /// The state property of the request that could not be found.
        /// </summary>
        public string State => _state;

        /// <inheritdoc/>
        public override string Message => $"Request with state {_state} has not been completed. Please ensure {nameof(OAuthFlow.AuthorizationCallback)} has been called.";

        /// <summary>
        /// Initializes a new instance of <see cref="OAuthRequestNotCompletedException"/> with the given <paramref name="state"/>.
        /// </summary>
        /// <param name="state"></param>
        public OAuthRequestNotCompletedException(string state)
        {
            _state = state;
        }

        private OAuthRequestNotCompletedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _state = info.GetString(nameof(State));
        }

        /// <summary>
        /// Immediately throws a <see cref="OAuthRequestNotCompletedException"/> with the given state as <see cref="State"/>.
        /// </summary>
        /// <param name="state"></param>
        /// <exception cref="OAuthRequestNotCompletedException"></exception>
        internal static void ThrowForState(string state)
        {
            throw new OAuthRequestNotCompletedException(state);
        }
    }
}
