using DG.Common.Http.Fluent;
using DG.Common.Http.Testing;
using FluentAssertions;
using NSubstitute;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DG.Common.Http.Tests.Fluent
{
    public class FluentRequestTests
    {
        [Theory]
        [InlineData("https://www.test.com/api/v1.0/", "users", "https://www.test.com/api/v1.0/users")]
        [InlineData("https://www.test.com/api/v1.0", "users", "https://www.test.com/api/users")]
        [InlineData("https://www.test.com/api/v1.0/", "/users", "https://www.test.com/users")]
        [InlineData("https://www.test.com/api/v1.0", "/users", "https://www.test.com/users")]
        public void WithBaseAddress_Combines(string baseAddress, string requestUri, string expected)
        {
            Uri baseUri = null;
            if (!string.IsNullOrEmpty(baseAddress))
            {
                baseUri = new Uri(baseAddress);
            }
            var request = FluentRequest.Get.To(requestUri);

            var message = request.MessageForBaseUri(baseUri);

            message.RequestUri.Should().Be(new Uri(expected));
        }

        [Fact]
        public void WithHeader_AddsToList()
        {
            var request = FluentRequest.Get.To("https://www.test.com")
                .WithHeader(FluentHeader.ContentLength(10));

            request.Headers.Should().ContainSingle();
            request.Headers.Single().Value.Should().Be("10");
        }

        [Fact]
        public void WithHeader_CreatesCopyOfList()
        {
            var originalRequest = FluentRequest.Get.To("https://www.test.com")
                .WithHeader(FluentHeader.ContentLength(10));

            var secondRequest = originalRequest.WithHeader(FluentHeader.Authorization("bearer 1234"));

            originalRequest.Headers.Should().ContainSingle();
            secondRequest.Headers.Should().HaveCount(2);
        }

        [Fact]
        public void WithAuthorization_AddsHeader()
        {
            var request = FluentRequest.Get.To("https://www.test.com")
                .WithAuthorization(FluentAuthorization.ForBearer("test"));

            request.Headers.Should().ContainSingle();
            request.Headers.Single().Value.Should().Be("Bearer test");
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

            responseWithoutHeader.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseWithHeader.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task WithContentHeader_AppliesHeader()
        {
            var mockedHandler = Substitute.For<ISimpleMockHandler>();
            mockedHandler.GetResponse(Arg.Any<SimpleMockRequest>()).Returns(SimpleMockResponse.BadRequest());
            mockedHandler.GetResponse(Arg.Is<SimpleMockRequest>(m => m.ContentHeaders.ContainsKey("Content-Length") && !m.Headers.ContainsKey("Content-Length"))).Returns(SimpleMockResponse.Ok());
            var client = mockedHandler.CreateNewClient();
            var requestWithoutHeader = FluentRequest.Post.To("https://www.test.com");
            var requestWithHeader = FluentRequest.Post.To("https://www.test.com")
                .WithContent(FluentFormContentBuilder.With("username", "test"))
                .WithHeader(FluentHeader.ContentLength(500));

            var responseWithoutHeader = await client.SendAsync(requestWithoutHeader);
            var responseWithHeader = await client.SendAsync(requestWithHeader);

            responseWithoutHeader.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseWithHeader.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
