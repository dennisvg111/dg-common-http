using System.Net.Http;

namespace DG.Common.Http.Testing
{
    public static class IMockRequestExtensions
    {
        public static HttpClient CreateNewClient(this IMockRequest mockRequest)
        {
            var handler = new MockableHandler(mockRequest);
            return new HttpClient(handler);
        }
    }
}
