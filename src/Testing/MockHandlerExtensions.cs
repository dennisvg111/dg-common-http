using System.Net.Http;

namespace DG.Common.Http.Testing
{
    /// <summary>
    /// Provides extension methods to easily create new instances of <see cref="HttpMessageHandler"/> and <see cref="HttpClient"/> based on <see cref="IMockHandler"/> and <see cref="ISimpleMockHandler"/>.
    /// </summary>
    public static class MockHandlerExtensions
    {
        /// <summary>
        /// Returns a new instance of a mocked implementation of <see cref="HttpMessageHandler"/> based on the given <see cref="IMockHandler"/>.
        /// </summary>
        /// <param name="mockRequest"></param>
        /// <returns></returns>
        public static HttpMessageHandler CreateNewHandler(this IMockHandler mockRequest)
        {
            return new MockHttpMessageHandler(mockRequest);
        }

        /// <summary>
        /// Returns a new instance of a mocked implementation of <see cref="HttpMessageHandler"/> based on the given <see cref="ISimpleMockHandler"/>.
        /// </summary>
        /// <param name="mockRequest"></param>
        /// <returns></returns>
        public static HttpMessageHandler CreateNewHandler(this ISimpleMockHandler mockRequest)
        {
            var handler = new SimpleMockHandler(mockRequest);
            return new MockHttpMessageHandler(handler);
        }

        /// <summary>
        /// Returns a new instance of a <see cref="HttpClient"/> using a mocked implementation of <see cref="HttpMessageHandler"/> based on the given <see cref="IMockHandler"/>.
        /// </summary>
        /// <param name="mockRequest"></param>
        /// <returns></returns>
        public static HttpClient CreateNewClient(this IMockHandler mockRequest)
        {
            var handler = mockRequest.CreateNewHandler();
            return new HttpClient(handler);
        }

        /// <summary>
        /// Returns a new instance of a <see cref="HttpClient"/> using a mocked implementation of <see cref="HttpMessageHandler"/> based on the given <see cref="ISimpleMockHandler"/>.
        /// </summary>
        /// <param name="mockRequest"></param>
        /// <returns></returns>
        public static HttpClient CreateNewClient(this ISimpleMockHandler mockRequest)
        {
            var handler = mockRequest.CreateNewHandler();
            return new HttpClient(handler);
        }
    }
}
