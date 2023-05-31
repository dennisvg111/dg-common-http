using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DG.Common.Http.Testing
{
    public class MockableHandler : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await Task.Run(() => MockSend(request));
        }

        public virtual HttpResponseMessage MockSend(HttpRequestMessage request)
        {
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotImplemented,
                Content = new StringContent("")
            };
        }
    }
}
