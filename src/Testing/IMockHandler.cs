using System.Net.Http;

namespace DG.Common.Http.Testing
{
    /// <summary>
    /// Provides an interface to mock against when testing with <see cref="HttpMessageHandler"/> or <see cref="HttpClient"/>.
    /// </summary>
    public interface IMockHandler
    {
        /// <summary>
        /// This method will be called by a <see cref="HttpClient"/>, and can be implemented using a mocking library.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        HttpResponseMessage GetResponse(HttpRequestMessage request);
    }
}
