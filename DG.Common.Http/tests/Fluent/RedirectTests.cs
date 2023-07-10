using DG.Common.Http.Fluent;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace DG.Common.Http.Tests.Fluent
{
    public class RedirectTests
    {
        [Fact]
        public async void RedirectsWithCookies()
        {
            var message = FluentRequest.Get.To("https://httpbin.org/cookies/set/cookieName/cookieValue");
            HttpClientHandler handler = new HttpClientHandler();
            //handler.AllowAutoRedirect = false;
            HttpClient client = new HttpClient(handler);

            var result = await client.SendMessageAsync(message);

            var content = await result.Content.ReadAsStringAsync();
            var detectedCookies = JsonConvert.DeserializeObject<CookieResult>(content);

            Assert.NotNull(detectedCookies.Cookies["cookieName"]);
            Assert.Equal("cookieValue", detectedCookies.Cookies["cookieName"]);
        }

        [Fact]
        public async void RedirectsTwice()
        {
            var message = FluentRequest.Get.To("https://httpbin.org/redirect-to?url=%3Furl%3Dnew-url");
            HttpClientHandler handler = new HttpClientHandler();
            //handler.AllowAutoRedirect = false;
            HttpClient client = new HttpClient(handler);

            var result = await client.SendMessageAsync(message);

            var actual = result.RequestMessage.RequestUri.AbsoluteUri;
            Assert.Equal("https://httpbin.org/new-url", actual);
        }
    }


    public class CookieResult
    {
        [JsonProperty("cookies")]
        public Dictionary<string, string> Cookies { get; set; }
    }
}
