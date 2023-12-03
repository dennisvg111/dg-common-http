using DG.Common.Http.Fluent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace DG.Common.Http.Testing
{
    /// <summary>
    /// This class represents a request to an <see cref="ISimpleMockHandler"/>.
    /// </summary>
    public class SimpleMockRequest
    {
        private readonly string _url;
        private readonly HttpMethod _method = HttpMethod.Get;
        private readonly string _content = string.Empty;
        private readonly Dictionary<string, FluentHeader> _headers = new Dictionary<string, FluentHeader>();
        private readonly Dictionary<string, FluentHeader> _contentHeaders = new Dictionary<string, FluentHeader>();

        /// <summary>
        /// The url for this request.
        /// </summary>
        public string Url => _url;

        /// <summary>
        /// The method to be used for this request. Default <see cref="HttpMethod.Get"/>.
        /// </summary>
        public HttpMethod Method => _method;

        /// <summary>
        /// The content for this request. Default is an empty string. Note that this will be ignored if <see cref="Method"/> is <see cref="HttpMethod.Get"/>.
        /// </summary>
        public string Content => _content;

        /// <summary>
        /// The headers for this request.
        /// </summary>
        public IReadOnlyDictionary<string, FluentHeader> Headers => _headers;

        /// <summary>
        /// The headers for the content of this request.
        /// </summary>
        public IReadOnlyDictionary<string, FluentHeader> ContentHeaders => _contentHeaders;

        internal SimpleMockRequest(string url, HttpMethod method, string content, IEnumerable<FluentHeader> headers, IEnumerable<FluentHeader> contentHeaders)
        {
            _url = url;
            _method = method;
            _content = content;
            _headers = headers.ToDictionary(h => h.Name, h => h);
            _contentHeaders = contentHeaders.ToDictionary(h => h.Name, h => h);
        }
    }
}
