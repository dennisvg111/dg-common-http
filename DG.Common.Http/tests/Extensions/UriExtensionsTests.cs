﻿using DG.Common.Http.Extensions;
using System;
using Xunit;

namespace DG.Common.Http.Tests.Extensions
{
    public class UriExtensionsTests
    {
        [Theory]
        [InlineData("https://www.test.com/path?query=value", "http://www.test2.com/path?query2=value2#chapter", "http://www.test2.com/path?query2=value2#chapter")]
        [InlineData("https://www.test.com/path?query=value", "//www.test2.com/path?query=value", "https://www.test2.com/path?query=value")]
        [InlineData("https://www.test.com/path?query=value", "/otherpath/path4?query2=value2", "https://www.test.com/otherpath/path4?query2=value2")]
        [InlineData("https://www.test.com/path", "?newquery=x", "https://www.test.com/path?newquery=x")]
        [InlineData("https://www.test.com/path#chapter2", "?newquery=x", "https://www.test.com/path?newquery=x#chapter2")]
        [InlineData("https://www.test.com/path", "newpath", "https://www.test.com/newpath")]
        [InlineData("https://www.test.com/path/deeper", "newpath", "https://www.test.com/path/newpath")]
        [InlineData("https://www.test.com/path#test", "newpath", "https://www.test.com/newpath#test")]
        [InlineData("https://www.test.com/path#test", "/newpath", "https://www.test.com/newpath#test")]
        [InlineData("https://www.test.com/path#test", "newpath#chapter2", "https://www.test.com/newpath#chapter2")]
        [InlineData("https://www.test.com/path#chapter1", "/otherpath", "https://www.test.com/otherpath#chapter1")]
        [InlineData("https://www.test.com/path#chapter1", "https://www.newtest.com/otherpath", "https://www.newtest.com/otherpath#chapter1")]
        [InlineData("https://www.test.com/path#chapter1", "https://www.newtest.com/otherpath#chapter2", "https://www.newtest.com/otherpath#chapter2")]


        [InlineData("https://www.example.com/blog/latest", "2020/zoo", "https://www.example.com/blog/2020/zoo")]
        [InlineData("https://httpbin.org/redirect-to", "test", "https://httpbin.org/test")]
        [InlineData("https://httpbin.org/redirect-to", "&other=false", "https://httpbin.org/&other=false")]
        public void CombineForRedirectLocation(string original, string location, string expected)
        {
            Uri originalUri = new Uri(original, UriKind.Absolute);
            Uri locationUri = new Uri(location, UriKind.RelativeOrAbsolute);

            Uri newLocation = originalUri.CombineForRedirectLocation(locationUri);

            string actual = newLocation.AbsoluteUri;

            Assert.Equal(expected, actual);
        }
    }
}
