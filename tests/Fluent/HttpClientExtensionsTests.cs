using DG.Common.Http.Fluent;
using DG.Common.Http.Testing;
using DG.Common.Http.Tests.TestUtilities;
using NSubstitute;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DG.Common.Http.Tests.Fluent
{
    public class HttpClientExtensionsTests
    {
        private HttpClient CreateClient()
        {
            var mockedHandler = Substitute.For<ISimpleMockHandler>();
            mockedHandler.GetResponse(Arg.Any<SimpleMockRequest>()).Returns(SimpleMockResponse.Ok());
            return mockedHandler.CreateNewClient();
        }

        [Fact]
        public void SendAsync_UsesConfigureAwaitFalse()
        {
            var client = CreateClient();
            var request = FluentRequest.Get.To("https://www.test.com");

            Func<Task<HttpResponseMessage>> task = () => HttpClientExtensions.SendAsync(client, request);

            ContextAsserts.AssertContextSwitched(task);
        }
    }
}
