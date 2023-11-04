﻿using System.Linq;
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
            string content = request.Content?.ReadAsStringAsync().GetAwaiter().GetResult() ?? string.Empty;
            var headers = request.Headers.Select(h => new SimpleMockHeader(h.Key, string.Join(";", h.Value))).ToArray();
            var simpleRequest = new SimpleMockRequest(request.RequestUri.ToString(), request.Method, content, headers);

            var response = _request.GetResponse(simpleRequest);

            return new HttpResponseMessage(response.StatusCode)
            {
                Content = new StringContent(response.Content)
            };
        }
    }
}
