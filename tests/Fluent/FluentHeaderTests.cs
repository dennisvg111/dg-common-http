using DG.Common.Http.Fluent;
using FluentAssertions;
using System;
using Xunit;

namespace DG.Common.Http.Tests.Fluent
{
    public class FluentHeaderTests
    {
        [Theory]
        [InlineData(0, 1, 10, "bytes 0-0/10")]
        [InlineData(3, 3, 10, "bytes 3-5/10")]
        [InlineData(3, null, 10, "bytes */10")]
        [InlineData(3, 3, null, "bytes 3-5/*")]
        public void ContentRange_Correct(int startIndex, int? bytes, int? totalBytes, string expected)
        {
            var header = FluentHeader.ContentRange(startIndex, bytes, totalBytes);

            header.IsContentHeader.Should().BeTrue();
            header.Name.Should().Be("Content-Range");
            header.Value.Should().Be(expected);
        }

        [Fact]
        public void ContentLength_IsContentHeader()
        {
            var header = FluentHeader.ContentLength(1024);

            header.IsContentHeader.Should().BeTrue();
            header.Name.Should().Be("Content-Length");
            header.Value.Should().Be("1024");
        }

        [Fact]
        public void Authorization_IsNotContentHeader()
        {
            var header = FluentHeader.Authorization("Bearer 1234");

            header.IsContentHeader.Should().BeFalse();
            header.Name.Should().Be("Authorization");
            header.Value.Should().Be("Bearer 1234");
        }

        [Fact]
        public void Referrer_CorrectName()
        {
            var header = FluentHeader.Referrer(new Uri("https://www.test.com"));

            header.IsContentHeader.Should().BeFalse();
            //The official standard is misspelled, make sure we have the same spelling.
            header.Name.Should().Be("Referer");
            header.Value.Should().Be("https://www.test.com/");
        }

        [Fact]
        public void DefaultConstructor_ShouldAlwaysApply()
        {
            var header = new FluentHeader("Any", "any");

            header.ShouldApply.Should().BeTrue();
            header.IsContentHeader.Should().BeFalse();
        }
    }
}
