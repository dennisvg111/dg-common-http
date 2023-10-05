using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DG.Common.Http.Testing
{
    /// <summary>
    /// An implemenation of <see cref="HttpMessageHandler"/> that gets responses to requests using a <see cref="IMockHandler"/> or <see cref="ISimpleMockHandler"/>.
    /// </summary>
    public sealed class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly IMockHandler _handler;

        internal MockHttpMessageHandler(IMockHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// <para>Gets the response of an HTTP request as an asynchronous operation.</para>
        /// <para>Note that the <paramref name="cancellationToken"/> is NOT used.</para>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The cancellation token. This parameter is NOT used.</param>
        /// <returns>Returns <see cref="Task{TResult}"/>. The task object representing the asynchronous operation.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await Task.Run(() => _handler.GetResponse(request));
        }
    }
}
