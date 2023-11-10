using System;
using System.Runtime.Serialization;

namespace DG.Common.Http.Authorization.OAuth2.Exceptions
{
    /// <summary>
    /// Represents errors that occur when an <see cref="OAuthRequest"/> cannot be found.
    /// </summary>
    [Serializable]
    public sealed class OAuthRequestNotFoundException : Exception
    {
        private readonly string _state;

        /// <summary>
        /// The state property of the request that could not be found.
        /// </summary>
        public string State => _state;

        /// <inheritdoc/>
        public override string Message => $"Request with state {_state} could not be found.";

        /// <summary>
        /// Initializes a new instance of <see cref="OAuthRequestNotFoundException"/> with the given <paramref name="state"/>.
        /// </summary>
        /// <param name="state"></param>
        public OAuthRequestNotFoundException(string state)
        {
            _state = state;
        }

        private OAuthRequestNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _state = info.GetString(nameof(State));
        }

        /// <summary>
        /// Immediately throws a <see cref="OAuthRequestNotFoundException"/> with the given state as <see cref="State"/>.
        /// </summary>
        /// <param name="state"></param>
        /// <exception cref="OAuthRequestNotFoundException"></exception>
        internal static void ThrowForState(string state)
        {
            throw new OAuthRequestNotFoundException(state);
        }
    }
}
