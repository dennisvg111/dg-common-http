using System.Net;

namespace DG.Common.Http.Testing
{
    /// <summary>
    /// This class represents a response from an <see cref="ISimpleMockHandler"/>.
    /// </summary>
    public class SimpleMockResponse
    {
        private readonly string _content = string.Empty;
        private readonly HttpStatusCode _statusCode = HttpStatusCode.OK;

        /// <summary>
        /// The string content of this response.
        /// </summary>
        public string Content => _content;

        /// <summary>
        /// The status code of this response.
        /// </summary>
        public HttpStatusCode StatusCode => _statusCode;

        /// <summary>
        /// Creates a new instance of <see cref="SimpleMockResponse"/>, with the given <paramref name="content"/> and <paramref name="statusCode"/>.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="statusCode"></param>
        public SimpleMockResponse(string content, HttpStatusCode statusCode)
        {
            _content = content;
            _statusCode = statusCode;
        }

        /// <summary>
        /// Creates a new instance of <see cref="SimpleMockResponse"/>, with no content and <see cref="HttpStatusCode.OK"/>.
        /// </summary>
        /// <returns></returns>
        public static SimpleMockResponse Ok()
        {
            return Ok(string.Empty);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SimpleMockResponse"/>, with the given <paramref name="content"/> and <see cref="HttpStatusCode.OK"/>.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static SimpleMockResponse Ok(string content)
        {
            return new SimpleMockResponse(content, HttpStatusCode.OK);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SimpleMockResponse"/>, with no content and <see cref="HttpStatusCode.BadRequest"/>.
        /// </summary>
        /// <returns></returns>
        public static SimpleMockResponse BadRequest()
        {
            return new SimpleMockResponse(string.Empty, HttpStatusCode.BadRequest);
        }
    }
}
