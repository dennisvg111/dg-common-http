using DG.Common.Http.Testing;
using NSubstitute;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DG.Common.Http.Tests.Examples
{
    public class MockHandlerExamples
    {
        [Fact]
        public async Task GetResponse_CanBeMocked()
        {
            var mockedHandler = Substitute.For<ISimpleMockHandler>();
            mockedHandler.GetResponse(Arg.Any<SimpleMockRequest>()).Returns(SimpleMockResponse.Ok());
            var client = mockedHandler.CreateNewClient();

            var result = await client.GetAsync("http://www.test.com");

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task GetResponse_Differentiate()
        {
            var mockedHandler = Substitute.For<ISimpleMockHandler>();
            mockedHandler.GetResponse(Arg.Is<SimpleMockRequest>(m => m.Method == HttpMethod.Post)).Returns(SimpleMockResponse.Ok());
            mockedHandler.GetResponse(Arg.Is<SimpleMockRequest>(m => m.Method == HttpMethod.Get)).Returns(SimpleMockResponse.BadRequest());
            var client = mockedHandler.CreateNewClient();

            var result1 = await client.PostAsync("http://www.test.com", new StringContent(string.Empty));
            var result2 = await client.GetAsync("http://www.test.com");

            Assert.Equal(HttpStatusCode.OK, result1.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, result2.StatusCode);
        }

        [Fact]
        public async Task GetResponse_CountCalls()
        {
            var mockedHandler = Substitute.For<ISimpleMockHandler>();
            mockedHandler.GetResponse(Arg.Any<SimpleMockRequest>()).Returns(SimpleMockResponse.Ok());
            var client = mockedHandler.CreateNewClient();

            await client.GetAsync("http://www.test.com");
            await client.GetAsync("http://www.test.com/test");

            mockedHandler.Received(2).GetResponse(Arg.Any<SimpleMockRequest>());
            mockedHandler.Received(1).GetResponse(Arg.Is<SimpleMockRequest>(r => r.Url == "http://www.test.com/test"));
        }
    }
}
