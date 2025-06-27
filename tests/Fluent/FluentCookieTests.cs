using DG.Common.Http.Fluent;
using FluentAssertions;
using System;
using Xunit;

namespace DG.Common.Http.Tests.Fluent
{
    public class FluentCookieTests
    {
        [Fact]
        public void ConvertToSetCookieHeader_Name_IsCorrect()
        {
            var cookie = new FluentCookie("test", "value", new Uri("http://example.com"));

            var header = cookie.ConvertToSetCookieHeader();

            header.Name.Should().Be("Set-Cookie");
        }
    }
}
