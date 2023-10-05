using System;
using System.Runtime.Serialization;

namespace DG.Common.Http.Exceptions
{
    [Serializable]
    public class HttpAuthorizationException : Exception
    {
        public HttpAuthorizationException(string message) : base(message) { }
        public HttpAuthorizationException(string message, Exception inner) : base(message, inner) { }
        protected HttpAuthorizationException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public static HttpAuthorizationException DuringRefresh(bool initial = false)
        {
            if (initial)
            {
                return new HttpAuthorizationException("Could not retrieve initial authorization header value.");
            }
            return new HttpAuthorizationException("Could not refresh authorization header value.");
        }

        public HttpAuthorizationException WithException(Exception inner)
        {
            return new HttpAuthorizationException(Message, inner);
        }
    }
}
