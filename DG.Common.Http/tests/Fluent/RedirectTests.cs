using DG.Common.Http.Cookies;
using DG.Common.Http.Fluent;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace DG.Common.Http.Tests.Fluent
{
    public class RedirectTests
    {
        private static HttpClientSettings _settings = HttpClientSettings.WithBaseAddress("https://httpbin.org")
            .WithoutRedirects()
            .WithoutCookies();

        [Fact]
        public async void LimitAutomaticRedirectsTo_Works()
        {
            var client = HttpClientProvider.ClientForSettings(_settings);
            var message = FluentRequest.Get.To("/redirect-to?url=%3Furl%3Dfinal-url")
                .LimitAutomaticRedirectsTo(1);

            var result = await client.SendMessageAsync(message);

            int statusCode = (int)result.StatusCode;
            Assert.InRange(statusCode, 300, 399);
            var actual = result.RequestMessage.RequestUri.AbsoluteUri;
            Assert.Equal("https://httpbin.org/redirect-to?url=final-url", actual);
        }

        [Fact]
        public async void Redirection_Works()
        {
            var client = HttpClientProvider.ClientForSettings(_settings);
            var message = FluentRequest.Get.To("/redirect-to?url=%3Furl%3Dfinal-url");

            var result = await client.SendMessageAsync(message);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);

            var actual = result.RequestMessage.RequestUri.AbsoluteUri;
            Assert.Equal("https://httpbin.org/final-url", actual);
        }

        [Fact]
        public async void CookieJar_Collects()
        {
            var client = HttpClientProvider.ClientForSettings(_settings);
            var message = FluentRequest.Get.To("/cookies/set?cookie1=value1&cookie2=value2").WithoutRedirects();
            var jar = new CookieJar();

            var result = await client.SendMessageAsync(message);
            jar.CollectFrom(result);
            result.EnsureSuccessStatusCode();

            var content = await result.Content.ReadAsStringAsync();

            var detectedCookies = JsonConvert.DeserializeObject<CookieResult>(content);

            Assert.True(detectedCookies.Cookies.ContainsKey("cookie1"), "Cookies did not contain cookie with name 'cookie1'");
            Assert.Equal("value1", detectedCookies.Cookies["cookie1"]);

            Assert.True(detectedCookies.Cookies.ContainsKey("cookie2"), "Cookies did not contain cookie with name 'cookie2'");
            Assert.Equal("value2", detectedCookies.Cookies["cookie2"]);
        }
    }


    public class CookieResult
    {
        [JsonProperty("cookies")]
        public Dictionary<string, string> Cookies { get; set; }
    }
}
