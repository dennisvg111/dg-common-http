using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DG.Common.Http.Testing
{
    public class MockableHandler : HttpMessageHandler
    {
        private readonly IMockRequest _request;

        public MockableHandler(IMockRequest request)
        {
            _request = request;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await Task.Run(() => _request.GetResponse(request));
        }
    }
}
