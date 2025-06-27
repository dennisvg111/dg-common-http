using DG.Common.Http.Cookies;
using FluentAssertions;
using System;
using Xunit;

namespace DG.Common.Http.Tests.Cookies
{
    public class CookieExtensionsTests
    {
        private static readonly Uri _defaultOriginUri = new Uri("https://example.com", UriKind.Absolute);
        private static readonly DateTimeOffset _received = DateTimeOffset.UtcNow;

        [Fact]
        public void IsExpiredOn_MaxAgeLessThanZero_ReturnsTrue()
        {
            string headerValue = $"id=a3fWa; Max-Age={-1}";
            Cookie.TryParse(headerValue, _received, _defaultOriginUri, out ICookie cookie);

            cookie.IsExpired().Should().BeTrue();
        }
    }
}
