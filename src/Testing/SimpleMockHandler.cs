using DG.Common.Http.Fluent;
using System.Linq;
using System.Net.Http;

namespace DG.Common.Http.Testing
{
    internal class SimpleMockHandler : IMockHandler
    {
        private readonly ISimpleMockHandler _request;
        public SimpleMockHandler(ISimpleMockHandler request)
        {
            _request = request;
        }

        public HttpResponseMessage GetResponse(HttpRequestMessage request)
        {
            string content = request.Content?.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult() ?? string.Empty;
            var headers = request.Headers?.Select(h => FluentHeader.WithName(h.Key).AndValue(string.Join(";", h.Value))) ?? Enumerable.Empty<FluentHeader>();
            var contentHeaders = request.Content?.Headers?.Select(h => FluentHeader.WithName(h.Key).AndValue(string.Join(";", h.Value))) ?? Enumerable.Empty<FluentHeader>();
            var simpleRequest = new SimpleMockRequest(request.RequestUri.ToString(), request.Method, content, headers, contentHeaders);

            var response = _request.GetResponse(simpleRequest);

            return new HttpResponseMessage(response.StatusCode)
            {
                Content = new StringContent(response.Content)
            };
        }
    }
}
