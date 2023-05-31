using DG.Common.Http.Testing;
using NSubstitute;
using System.Net;
using System.Net.Http;
using Xunit;

namespace DG.Common.Http.Tests.Examples
{
    public class MockHttpMessageHandlerExamples
    {
        [Fact]
        public async void Unmocked_ReturnsNotImplemented()
        {
            var client = new HttpClient(new MockableHandler());

            var result = await client.GetAsync("http://www.test.com");

            Assert.Equal(HttpStatusCode.NotImplemented, result.StatusCode);
        }

        [Fact]
        public async void MockSend_ReturnsOk()
        {
            var statusOk = new System.Net.Http.HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK
            };
            var mockedHandler = Substitute.ForPartsOf<MockableHandler>();
            mockedHandler.MockSend(Arg.Any<HttpRequestMessage>()).Returns(statusOk);

            var client = new HttpClient(mockedHandler);

            var result = await client.GetAsync("http://www.test.com");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async void MockSend_CheckRequest()
        {
            var statusOk = new System.Net.Http.HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK
            };
            var statusBadRequest = new System.Net.Http.HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest
            };
            var mockedHandler = Substitute.ForPartsOf<MockableHandler>();
            mockedHandler.MockSend(Arg.Is<HttpRequestMessage>(m => m.RequestUri.Host == "www.test.com")).Returns(statusOk);
            mockedHandler.MockSend(Arg.Is<HttpRequestMessage>(m => m.RequestUri.Host != "www.test.com")).Returns(statusBadRequest);

            var client = new HttpClient(mockedHandler);

            var result1 = await client.GetAsync("http://www.test.com");
            var result2 = await client.GetAsync("http://www.other-site.com");

            Assert.Equal(HttpStatusCode.OK, result1.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, result2.StatusCode);
        }

        [Fact]
        public async void MockSend_AmountOfCalls()
        {
            var statusOk = new System.Net.Http.HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK
            };
            var mockedHandler = Substitute.ForPartsOf<MockableHandler>();
            mockedHandler.MockSend(Arg.Any<HttpRequestMessage>()).Returns(statusOk);

            var client = new HttpClient(mockedHandler);

            await client.GetAsync("http://www.test.com");
            await client.GetAsync("http://www.test.com/test");

            mockedHandler.Received(2).MockSend(Arg.Any<HttpRequestMessage>());
        }
    }
}
