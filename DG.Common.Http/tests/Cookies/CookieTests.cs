using DG.Common.Http.Cookies;
using DG.Common.Http.Extensions;
using System;
using Xunit;

namespace DG.Common.Http.Tests.Cookies
{
    public class CookieTests
    {
        private readonly Uri _defaultOriginUri = new Uri("https://example.com", UriKind.Absolute);
        private readonly DateTimeOffset _received = DateTimeOffset.UtcNow;

        [Fact]
        public void TryParse_OnlyNameValue()
        {
            string headerValue = $"sessionId=38afes7a8";

            Assert.True(Cookie.TryParse(headerValue, _received, _defaultOriginUri, out Cookie cookie));

            Assert.Equal("sessionId", cookie.Name);
            Assert.Equal("38afes7a8", cookie.Value);
            Assert.False(cookie.IsExpired(), "Cookie should not be expired.");
            Assert.True(cookie.IsSessionCookie, "Cookie should be marked as session cookie.");
        }

        public static object[][] ExpiresData = new object[][]
        {
            new object[] { DateTimeOffset.UtcNow.AddHours(1), false },
            new object[] { DateTimeOffset.UtcNow.AddHours(-1), true }
        };
        [Theory]
        [MemberData(nameof(ExpiresData))]
        public void TryParse_Expires(DateTimeOffset expirationDate, bool isExpired)
        {
            string cookieExpires = expirationDate.ToCookieExpiresString();
            string headerValue = $"id=a3fWa; Expires={cookieExpires}";

            Assert.True(Cookie.TryParse(headerValue, expirationDate.AddDays(-1), _defaultOriginUri, out Cookie cookie));

            Assert.Equal("id", cookie.Name);
            Assert.Equal("a3fWa", cookie.Value);
            Assert.True(cookie.IsExpired() == isExpired, $"cookie.{nameof(isExpired)}() should return {isExpired}.");
            Assert.False(cookie.IsSessionCookie, "Cookie should not be marked as session cookie.");
        }

        public static object[][] MaxAgeData = new object[][]
        {
            new object[] { 1000, false },
            new object[] { 0, true },
            new object[] { -50, true }
        };
        [Theory]
        [MemberData(nameof(MaxAgeData))]
        public void TryParse_MaxAge(int maxAge, bool isExpired)
        {
            string headerValue = $"id=a3fWa; Max-Age={maxAge}";

            Assert.True(Cookie.TryParse(headerValue, _received, _defaultOriginUri, out Cookie cookie));

            Assert.Equal("id", cookie.Name);
            Assert.Equal("a3fWa", cookie.Value);
            Assert.True(cookie.IsExpired() == isExpired, $"cookie.{nameof(isExpired)}() should return {isExpired}.");
            Assert.False(cookie.IsSessionCookie, "Cookie should not be marked as session cookie.");
        }

        [Fact]
        public void IsValid_InValidDomain()
        {
            string headerValue = $"qwerty=219ffwef9w0f; Domain=somecompany.co.uk";
            Uri origin = new Uri("https://originalcompany.com", UriKind.Absolute);

            Assert.True(Cookie.TryParse(headerValue, _received, origin, out Cookie cookie));

            Assert.False(cookie.IsValid(out string reason));
        }

        [Fact]
        public void IsValid_HigherDomain()
        {
            string headerValue = $"sessionId=e8bb43229de9; Domain=foo.example.com";
            Uri origin = new Uri("https://example.com", UriKind.Absolute);

            Assert.True(Cookie.TryParse(headerValue, _received, origin, out Cookie cookie));

            Assert.False(cookie.IsValid(out string reason));
        }

        [Fact]
        public void IsValid_InvalidTld()
        {
            string headerValue = $"qwerty=219ffwef9w0f; Domain=.com";
            Uri origin = new Uri("https://originalcompany.com", UriKind.Absolute);

            Assert.True(Cookie.TryParse(headerValue, _received, origin, out Cookie cookie));

            Assert.False(cookie.IsValid(out string reason));
        }

        [Fact]
        public void IsValid_InvalidIp()
        {
            string headerValue = $"ip=test; Domain=192.168.127.108";
            Uri origin = new Uri("http://192.168.127.108", UriKind.Absolute);

            Assert.True(Cookie.TryParse(headerValue, _received, origin, out Cookie cookie));

            Assert.False(cookie.IsValid(out string reason));
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
        }
    }
}
