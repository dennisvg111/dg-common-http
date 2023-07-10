using DG.Common.Http.Extensions;
using System;
using Xunit;

namespace DG.Common.Http.Tests.Extensions
{
    public class UriExtensionsTests
    {
        [Theory]
        //absolute urls
        [InlineData("https://www.test.com/path?query=value", "http://www.test2.com/path?query2=value2#chapter", "http://www.test2.com/path?query2=value2#chapter")]

        //scheme preservation
        [InlineData("https://www.test.com/path?query=value", "//www.test2.com/path?query=value", "https://www.test2.com/path?query=value")]
        [InlineData("http://www.test.com/path?query=value", "//www.test2.com/path?query=value", "http://www.test2.com/path?query=value")]

        //relative url absolute path
        [InlineData("https://www.test.com/path", "/otherpath", "https://www.test.com/otherpath")]
        [InlineData("https://www.test.com/path/deeper", "/otherpath", "https://www.test.com/otherpath")]
        [InlineData("https://www.test.com/path/deeper?query=value", "/otherpath", "https://www.test.com/otherpath")]
        [InlineData("https://www.test.com/path?query=value", "/otherpath/path4?query2=value2", "https://www.test.com/otherpath/path4?query2=value2")]

        //query location
        [InlineData("https://www.test.com/path", "?newquery=x", "https://www.test.com/path?newquery=x")]

        //relative url relative path
        [InlineData("https://www.test.com/path", "newpath", "https://www.test.com/newpath")]
        [InlineData("https://www.test.com/path/deeper", "newpath", "https://www.test.com/path/newpath")]

        //fragment preservation
        [InlineData("https://www.test.com/path#chapter1", "https://www.newtest.com/otherpath", "https://www.newtest.com/otherpath#chapter1")]
        [InlineData("https://www.test.com/path/deeper#chapter1", "/newpath", "https://www.test.com/newpath#chapter1")]
        [InlineData("https://www.test.com/path/deeper#chapter1", "newpath", "https://www.test.com/path/newpath#chapter1")]
        [InlineData("https://www.test.com/path#chapter1", "?newquery=x", "https://www.test.com/path?newquery=x#chapter1")]

        //fragment overwrite
        [InlineData("https://www.test.com/path#chapter1", "#chapter2", "https://www.test.com/path#chapter2")]
        [InlineData("https://www.test.com/path#chapter1", "/newpath#chapter2", "https://www.test.com/newpath#chapter2")]
        [InlineData("https://www.test.com/path#chapter1", "https://www.newtest.com/otherpath#chapter2", "https://www.newtest.com/otherpath#chapter2")]

        //non-standard ports
        [InlineData("https://www.test.com:81/places/path", "newpath", "https://www.test.com:81/places/newpath")]
        [InlineData("https://www.test.com:81/places/path", "/newpath", "https://www.test.com:81/newpath")]
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
