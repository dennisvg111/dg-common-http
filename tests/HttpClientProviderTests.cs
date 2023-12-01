using FluentAssertions;
using Xunit;

namespace DG.Common.Http.Tests
{
    public class HttpClientProviderTests
    {
        [Fact]
        public void CreateNewClientForSettings_SameSettings_ReturnsNewInstance()
        {
            var settings = HttpClientSettings.WithBaseAddress("https://www.test.com");

            var client1 = HttpClientProvider.CreateNewClientForSettings(settings);
            var client2 = HttpClientProvider.CreateNewClientForSettings(settings);

            client1.Should().NotBeSameAs(client2);
        }

        [Fact]
        public void CreateNewClientForSettings_SameSettings_ReturnsSameInstance()
        {
            var settings1 = HttpClientSettings.WithBaseAddress("https://www.test.com");
            var settings2 = HttpClientSettings.WithBaseAddress("https://www.test.com");

            var client1 = HttpClientProvider.ClientForSettings(settings1);
            var client2 = HttpClientProvider.ClientForSettings(settings2);

            client1.Should().BeSameAs(client2);
        }

        [Fact]
        public void CreateNewClientForSettings_DifferentSettings_ReturnsNewInstance()
        {
            var settings1 = HttpClientSettings.WithBaseAddress("https://www.test.com");
            var settings2 = HttpClientSettings.WithBaseAddress("https://www.other-test.com");

            var client1 = HttpClientProvider.ClientForSettings(settings1);
            var client2 = HttpClientProvider.ClientForSettings(settings2);

            client1.Should().NotBeSameAs(client2);
        }
    }
}
