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
        public async void TestRedirects()
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
    }


    public class CookieResult
    {
        [JsonProperty("cookies")]
        public Dictionary<string, string> Cookies { get; set; }
    }
}
