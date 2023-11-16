using System;
using System.Runtime.Serialization;

namespace DG.Common.Http.Authorization.OAuth2.Exceptions
{
    /// <summary>
    /// Represents errors that occur when an <see cref="OAuthFlow"/> cannot be found.
    /// </summary>
    [Serializable]
    public sealed class OAuthFlowNotFoundException : Exception
    {
        private readonly string _state;

        /// <summary>
        /// The state value of the authorization flow that could not be found.
        /// </summary>
        public string State => _state;

        /// <inheritdoc/>
        public override string Message => $"Request with state {_state} could not be found.";

        /// <summary>
        /// Initializes a new instance of <see cref="OAuthFlowNotFoundException"/> with the given <paramref name="state"/>.
        /// </summary>
        /// <param name="state"></param>
        public OAuthFlowNotFoundException(string state)
        {
            _state = state;
        }

        private OAuthFlowNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _state = info.GetString(nameof(State));
        }

        /// <summary>
        /// Immediately throws a <see cref="OAuthFlowNotFoundException"/> with the given state as <see cref="State"/>.
        /// </summary>
        /// <param name="state"></param>
        /// <exception cref="OAuthFlowNotFoundException"></exception>
        internal static void ThrowForState(string state)
        {
            throw new OAuthFlowNotFoundException(state);
        }
    }
}
