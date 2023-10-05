using DG.Common.Http.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DG.Common.Http.Tests.Fluent
{
    public class UriBuilderExtensionsTests
    {
        [Fact]
        public void GetQueryParts_ReturnsList()
        {
            UriBuilder builder = new UriBuilder("https://www.test.com/login.aspx?test=123&test=abc");

            var queryParts = builder.GetQueryParts();

            Assert.NotNull(queryParts);
            Assert.Equal(2, queryParts.Count());
            Assert.Contains(queryParts, (kv) => kv.Key == "test" && kv.Value == "123");
            Assert.Contains(queryParts, (kv) => kv.Key == "test" && kv.Value == "abc");
        }

        [Fact]
        public void GetQueryParts_Unescapes()
        {
            UriBuilder builder = new UriBuilder("https://www.test.com/login.aspx?test1=%25&test2=%3D");

            var queryParts = builder.GetQueryParts();

            Assert.NotNull(queryParts);
            Assert.NotEmpty(queryParts);
            Assert.Contains(queryParts, (kv) => kv.Key == "test1" && kv.Value == "%");
            Assert.Contains(queryParts, (kv) => kv.Key == "test2" && kv.Value == "=");
        }

        [Fact]
        public void GetQueryParts_ReturnsSingle()
        {
            UriBuilder builder = new UriBuilder("https://www.test.com/login.aspx?test=123");

            var queryParts = builder.GetQueryParts();

            Assert.NotNull(queryParts);
            Assert.Single(queryParts);
            Assert.Contains(queryParts, (kv) => kv.Key == "test" && kv.Value == "123");
        }

        [Fact]
        public void GetQueryParts_ReturnsEmpty()
        {
            UriBuilder builder = new UriBuilder("https://www.test.com/login.aspx");

            var queryParts = builder.GetQueryParts();

            Assert.NotNull(queryParts);
            Assert.Empty(queryParts);
        }

        [Fact]
        public void SetQueryParts_Replaces()
        {
            UriBuilder builder = new UriBuilder("https://www.test.com/login.aspx?unused=test");
            var queryParts = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("test", "123"),
                new KeyValuePair<string, string>("test", "abc")
            };

            builder.SetQueryParts(queryParts);
            string uri = builder.Uri.ToString();

            Assert.Equal("https://www.test.com/login.aspx?test=123&test=abc", uri);
        }

        [Fact]
        public void SetQueryParts_Escapes()
        {
            UriBuilder builder = new UriBuilder("https://www.test.com/login.aspx");
            var queryParts = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("test", "%25"),
                new KeyValuePair<string, string>("test", "123="),
                new KeyValuePair<string, string>("test", "a&bc")
            };

            builder.SetQueryParts(queryParts);
            string uri = builder.Uri.ToString();

            Assert.Equal("https://www.test.com/login.aspx?test=%2525&test=123%3D&test=a%26bc", uri);
        }

        [Fact]
        public void WithQueryParts_Adds()
        {
            string originalUri = "https://www.test.com/login.aspx?used=test";
            UriBuilder builder = new UriBuilder(originalUri);
            var queryParts = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("test", null),
                new KeyValuePair<string, string>("test", "abc")
            };

            var clone = builder.WithQueryParts(queryParts);
            string uri = clone.Uri.ToString();

            Assert.Equal("https://www.test.com/login.aspx?used=test&test&test=abc", uri);
            Assert.Equal(originalUri, builder.Uri.ToString());
        }
    }
}
