using DG.Common.Http.Cookies;
using DG.Common.Http.Fluent;
using Xunit;

namespace DG.Common.Http.Tests.Fluent
{
    public class FluentFormContentTests
    {
        private static HttpClientSettings _settings = HttpClientSettings.WithBaseAddress("https://www.dennisvg.nl")
            .WithoutRedirects()
            .WithCookies();


        [Fact]
        public async void CookieJar_Collects()
        {
            var client = HttpClientProvider.ClientForSettings(_settings);
            await client.SendMessageAsync(FluentRequest.Get.To("/"));
            var message = FluentRequest.Post.To("/account/validate")
                .WithContent(FluentFormContent
                    .With("username", "Grimm")
                    .AndWith("password", "")
                    .AndWith("rememberMe", "true")
                );
            var jar = new CookieJar();

            var result = await client.SendMessageAsync(message);
            jar.CollectFrom(result);
            result.EnsureSuccessStatusCode();

            var content = await result.Content.ReadAsStringAsync();
        }
    }
}
