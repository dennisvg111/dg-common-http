using DG.Common.Http.Cookies;
using System;
using Xunit;

namespace DG.Common.Http.Tests.Cookies
{
    public class CookieRuleTests
    {
        private readonly Uri _defaultOriginUri = new Uri("https://example.com", UriKind.Absolute);
        private readonly DateTimeOffset _received = DateTimeOffset.UtcNow;

        [Fact]
        public void IsValid_InValidDomain()
        {
            string headerValue = $"qwerty=219ffwef9w0f; Domain=somecompany.co.uk";
            Uri origin = new Uri("https://originalcompany.com", UriKind.Absolute);

            Assert.True(Cookie.TryParse(headerValue, _received, origin, out Cookie cookie));

            Assert.False(cookie.IsValid(out string reason));
            Assert.Contains("domain", reason, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void IsValid_DomainRelativePath()
        {
            string headerValue = $"qwerty=219ffwef9w0f; Domain=example.com/path";

            Assert.True(Cookie.TryParse(headerValue, _received, _defaultOriginUri, out Cookie cookie));

            Assert.False(cookie.IsValid(out string reason));
            Assert.Contains("domain", reason, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void IsValid_HigherDomain()
        {
            string headerValue = $"sessionId=e8bb43229de9; Domain=foo.example.com";
            Uri origin = new Uri("https://example.com", UriKind.Absolute);

            Assert.True(Cookie.TryParse(headerValue, _received, origin, out Cookie cookie));

            Assert.False(cookie.IsValid(out string reason));
            Assert.Contains("domain", reason, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void IsValid_InvalidTld()
        {
            string headerValue = $"qwerty=219ffwef9w0f; Domain=.com";
            Uri origin = new Uri("https://originalcompany.com", UriKind.Absolute);

            Assert.True(Cookie.TryParse(headerValue, _received, origin, out Cookie cookie));

            Assert.False(cookie.IsValid(out string reason));
            Assert.Contains("domain", reason, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void IsValid_InvalidIp()
        {
            string headerValue = $"ip=test; Domain=192.168.127.108";
            Uri origin = new Uri("http://192.168.127.108", UriKind.Absolute);

            Assert.True(Cookie.TryParse(headerValue, _received, origin, out Cookie cookie));

            Assert.False(cookie.IsValid(out string reason));
            Assert.Contains("IP address", reason, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData(SameSitePolicy.Lax, false, true)]
        [InlineData(SameSitePolicy.Lax, true, true)]
        [InlineData(SameSitePolicy.Strict, false, true)]
        [InlineData(SameSitePolicy.Strict, true, true)]
        [InlineData(SameSitePolicy.None, false, false)]
        [InlineData(SameSitePolicy.None, true, true)]
        public void IsValid_SameSiteSecure(SameSitePolicy policy, bool secure, bool expected)
        {
            string headerValue = $"ip=test; SameSite={policy}";
            if (secure)
            {
                headerValue += "; Secure";
            }

            Assert.True(Cookie.TryParse(headerValue, _received, _defaultOriginUri, out Cookie cookie));

            var actual = cookie.IsValid(out string reason);
            Assert.True(actual == expected, $"Cookie should{(expected ? "" : " not")} be validated.");
            if (!actual)
            {
                Assert.Contains("Secure", reason, StringComparison.OrdinalIgnoreCase);
                Assert.Contains("SameSite", reason, StringComparison.OrdinalIgnoreCase);
            }
        }

        [Theory]
        [InlineData("__Host-cookie=test", false)]
        [InlineData("__Host-cookie=test; Secure", false)]
        [InlineData("__Host-cookie=test; Secure; Path=/", true)]
        [InlineData("__Host-cookie=test; Secure; Path=/test", false)]
        [InlineData("__Host-cookie=test; Secure; Domain=.example.com", false)]
        [InlineData("__Host-cookie=test; Secure; Domain=.example.com; Path=/", false)]
        [InlineData("__Host-cookie=test; Secure; Domain=.example.com; Path=/test", false)]
        public void IsValid_HostPrefix(string headerValue, bool expectedIsValid)
        {
            Assert.True(Cookie.TryParse(headerValue, _received, _defaultOriginUri, out Cookie cookie));

            var actual = cookie.IsValid(out string reason);
            Assert.True(actual == expectedIsValid, $"Cookie should{(expectedIsValid ? "" : " not")} be validated.");
            if (!actual)
            {
                Assert.Contains("__Host", reason, StringComparison.OrdinalIgnoreCase);
            }
        }

        [Theory]
        [InlineData("__Secure-cookie=test", false)]
        [InlineData("__Secure-cookie=test; Secure", true)]
        [InlineData("__Secure-cookie=test; Secure; Path=/", true)]
        [InlineData("__Secure-cookie=test; Path=/test", false)]
        [InlineData("__Secure-cookie=test; Secure; Domain=.example.com", true)]
        [InlineData("__Secure-cookie=test; Secure; Domain=.example.com; Path =/ ", true)]
        [InlineData("__Secure-cookie=test; Domain=.example.com; Path=/", false)]
        public void IsValid_SecurePrefix(string headerValue, bool expectedIsValid)
        {
            Assert.True(Cookie.TryParse(headerValue, _received, _defaultOriginUri, out Cookie cookie));

            var actual = cookie.IsValid(out string reason);
            Assert.True(actual == expectedIsValid, $"Cookie should{(expectedIsValid ? "" : " not")} be validated.");
            if (!actual)
            {
                Assert.Contains("__Secure", reason, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
