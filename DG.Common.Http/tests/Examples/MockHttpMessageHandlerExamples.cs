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
        public async void MockSend_ReturnsOk()
        {
            var statusOk = new System.Net.Http.HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK
            };
            var mockedHandler = Substitute.For<IMockRequest>();
            mockedHandler.GetResponse(Arg.Any<HttpRequestMessage>()).Returns(statusOk);
            var client = mockedHandler.CreateNewClient();

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
            var mockedHandler = Substitute.For<IMockRequest>();
            mockedHandler.GetResponse(Arg.Is<HttpRequestMessage>(m => m.Method == HttpMethod.Post)).Returns(statusOk);
            mockedHandler.GetResponse(Arg.Is<HttpRequestMessage>(m => m.Method == HttpMethod.Get)).Returns(statusBadRequest);
            var client = mockedHandler.CreateNewClient();

            var result1 = await client.PostAsync("http://www.test.com", new StringContent(string.Empty));
            var result2 = await client.GetAsync("http://www.test.com");

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
            var mockedHandler = Substitute.For<IMockRequest>();
            mockedHandler.GetResponse(Arg.Any<HttpRequestMessage>()).Returns(statusOk);
            var client = mockedHandler.CreateNewClient();

            await client.GetAsync("http://www.test.com");
            await client.GetAsync("http://www.test.com/test");

            mockedHandler.Received(2).GetResponse(Arg.Any<HttpRequestMessage>());
        }
    }
}
