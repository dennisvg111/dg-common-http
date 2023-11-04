using DG.Common.Http.Fluent;
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

            Assert.Equal("Content-Range", header.Name);
            Assert.Equal(expected, header.Value);
            Assert.True(header.IsContentHeader);
        }

        [Fact]
        public void ContentLength_IsContentHeader()
        {
            var header = FluentHeader.ContentLength(1024);

            Assert.Equal("Content-Length", header.Name);
            Assert.True(header.IsContentHeader);
        }

        [Fact]
        public void Authorization_IsNotContentHeader()
        {
            var header = FluentHeader.Authorization("Bearer 1234");

            Assert.Equal("Authorization", header.Name);
            Assert.False(header.IsContentHeader);
        }

        [Fact]
        public void Referrer_CorrectName()
        {
            //The official standard is misspelled, make sure we have the same spelling.
            var header = FluentHeader.Referrer(new Uri("https://www.test.com"));

            Assert.Equal("Referer", header.Name);
            Assert.False(header.IsContentHeader);
        }

        [Fact]
        public void DefaultConstructor_ShouldAlwaysApply()
        {
            var header = new FluentHeader("any", "any");

            Assert.True(header.ShouldApply);
            Assert.False(header.IsContentHeader);
        }
    }
}
