using System.Net.Http;

namespace DG.Common.Http.Testing
{
    public interface IMockRequest
    {
        HttpResponseMessage GetResponse(HttpRequestMessage request);
    }
}
