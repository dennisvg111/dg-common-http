using DG.Common.Http.Fluent;
using System;
using System.Net.Http;
using Xunit;

namespace DG.Common.Http.Tests.Fluent
{
    public class RedirectTests
    {
        [Fact]
        public async void TestRedirects()
        {
            var message = FluentRequest.Get.To("https://httpbin.org/cookies/set/cookieName/cookieValue");
            HttpClientHandler handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);

            var result = await client.SendMessageAsync(message);
            var cookies = handler.CookieContainer.GetCookies(new Uri("https://httpbin.org"));

            Assert.Single(cookies);
            Assert.NotNull(cookies["cookieName"]);
            Assert.Equal("cookieValue", cookies["cookieName"].Value);
        }
    }
}
