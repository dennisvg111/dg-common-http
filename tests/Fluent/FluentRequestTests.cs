using DG.Common.Http.Fluent;
using DG.Common.Http.Testing;
using NSubstitute;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DG.Common.Http.Tests.Fluent
{
    public class FluentRequestTests
    {
        [Fact]
        public void WithHeader_AddsToList()
        {
            var request = FluentRequest.Get.To("https://www.test.com")
                .WithHeader(FluentHeader.ContentLength(10));

            Assert.Single(request.Headers);
            Assert.Equal("10", request.Headers.Single().Value);
        }

        [Fact]
        public void WithHeader_CreatesCopyOfList()
        {
            var originalRequest = FluentRequest.Get.To("https://www.test.com")
                .WithHeader(FluentHeader.ContentLength(10));

            var secondRequest = originalRequest.WithHeader(FluentHeader.Authorization("bearer 1234"));

            Assert.Equal(2, secondRequest.Headers.Count);
            Assert.Single(originalRequest.Headers);
        }

        [Fact]
        public async Task WithHeader_AppliesHeader()
        {
            var mockedHandler = Substitute.For<ISimpleMockHandler>();
            mockedHandler.GetResponse(Arg.Any<SimpleMockRequest>()).Returns(SimpleMockResponse.BadRequest());
            mockedHandler.GetResponse(Arg.Is<SimpleMockRequest>(m => m.Headers.ContainsKey("Authorization"))).Returns(SimpleMockResponse.Ok());
            var client = mockedHandler.CreateNewClient();
            var requestWithoutHeader = FluentRequest.Post.To("https://www.test.com");
            var requestWithHeader = FluentRequest.Post.To("https://www.test.com")
                .WithHeader(FluentHeader.Authorization("bearer 1234"));

            var responseWithoutHeader = await client.SendAsync(requestWithoutHeader);
            var responseWithHeader = await client.SendAsync(requestWithHeader);

            Assert.Equal(HttpStatusCode.BadRequest, responseWithoutHeader.StatusCode);
            Assert.Equal(HttpStatusCode.OK, responseWithHeader.StatusCode);
        }
    }
}
