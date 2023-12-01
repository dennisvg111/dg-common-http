using DG.Common.Http.Fluent;
using FluentAssertions;
using System.Net.Http;
using Xunit;

namespace DG.Common.Http.Tests.Fluent
{
    public class FluentFormContentBuilderTests
    {
        [Fact]
        public void BuildUrlEncodedForm_MediaType_Correct()
        {
            var form = FluentFormContentBuilder
                .With("username", "user")
                .AndWith("password", "MyPassword123!")
                .BuildUrlEncodedForm();

            form.Headers.ContentType.MediaType.Should().Be("application/x-www-form-urlencoded");
        }

        [Fact]
        public void BuildMultipartForm_MediaType_Correct()
        {
            var form = FluentFormContentBuilder
                .With("username", "user")
                .AndWith("file", new ByteArrayContent(new byte[] { 1, 2 }))
                .AndWith("password", "MyPassword123!")
                .BuildMultipartForm();

            form.Headers.ContentType.MediaType.Should().Be("multipart/form-data");
        }
    }
}
