﻿using DG.Common.Caching;
using DG.Common.Caching.Memory;
using DG.Common.Http.Cookies;
using DG.Common.Http.Fluent;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Xunit;

namespace DG.Common.Http.Tests.Fluent
{
    public class RedirectTests
    {
        private static HttpClientProvider _httpClientProvider = new HttpClientProvider(new TypedCacheProvider(TypedMemoryCacheFactory.Default));
        private static HttpClientSettings _settings = HttpClientSettings.WithBaseAddress("https://httpbin.org")
            .WithoutRedirects()
            .WithoutCookies();

        [Fact]
        public async void LimitAutomaticRedirectsTo_Works()
        {
            var client = _httpClientProvider.ClientForSettings(_settings);
            var request = FluentRequest.Get.To("/redirect-to?url=%3Furl%3Dfinal-url")
                .LimitAutomaticRedirectsTo(1);

            var result = await client.SendAsync(request);

            int statusCode = (int)result.StatusCode;
            Assert.InRange(statusCode, 300, 399);
            var actual = result.RequestMessage.RequestUri.AbsoluteUri;
            Assert.Equal("https://httpbin.org/redirect-to?url=final-url", actual);
        }

        [Fact]
        public async void Redirection_Works()
        {
            var client = _httpClientProvider.ClientForSettings(_settings);
            var request = FluentRequest.Get.To("/redirect-to?url=%3Furl%3Dfinal-url");

            var result = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);

            var actual = result.RequestMessage.RequestUri.AbsoluteUri;
            Assert.Equal("https://httpbin.org/final-url", actual);
        }

        [Fact]
        public async void CookieJar_Collects()
        {
            var client = _httpClientProvider.ClientForSettings(_settings);
            var jar = new CookieJar();

            var result = new HttpResponseMessage(HttpStatusCode.Redirect);
            result.RequestMessage = new HttpRequestMessage(HttpMethod.Post, new System.Uri("https://httpbin.org/cookies/set", System.UriKind.Absolute));
            result.Headers.Add("Set-Cookie", new string[] { "cookie1=value1; Path=/", "cookie2=value2; Path=/" });
            jar.CollectFrom(result);

            var request = FluentRequest.Get.To("/cookies").WithCookieJar(jar);
            result = await client.SendAsync(request);
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
