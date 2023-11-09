using DG.Common.Http.Fluent;
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

            Assert.Equal("application/x-www-form-urlencoded", form.Headers.ContentType.MediaType);
        }

        [Fact]
        public void BuildMultipartForm_MediaType_Correct()
        {
            var form = FluentFormContentBuilder
                .With("username", "user")
                .AndWith("file", new ByteArrayContent(new byte[] { 1, 2 }))
                .AndWith("password", "MyPassword123!")
                .BuildMultipartForm();

            Assert.Equal("multipart/form-data", form.Headers.ContentType.MediaType);
        }
    }
}
