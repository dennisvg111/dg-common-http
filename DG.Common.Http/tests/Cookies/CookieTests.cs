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
    }
}
