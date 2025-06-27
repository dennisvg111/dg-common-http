using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace DG.Common.Http.DependencyInjection.Tests;
public class ScopeAwareHttpClientBuilderExtensionsTests
{
    private static IServiceCollection BuildDefaultClientFactoryServices()
    {
        var services = new ServiceCollection();
        services.AddHttpClient("TestClient")
            .AddHttpMessageHandler<ScopedMessageHander>();

        services.AddTransient<ScopedMessageHander>();
        services.AddScoped<ScopedService>();

        return services;
    }
    private static IServiceCollection BuildScopeAwareClientFactoryServices()
    {
        var services = new ServiceCollection();
        services.AddHttpClient("TestClient")
            .AddScopeAwareHttpHandler<ScopedMessageHander>();

        services.AddTransient<ScopedMessageHander>();
        services.AddScoped<ScopedService>();

        return services;
    }

    [Fact(Skip = "This is an example, this test is expected to fail.")]
    public async Task EXAMPLE_AddDefaultHttpClient_DoesNotReuseScopedServiceInHandler()
    {
        var services = BuildDefaultClientFactoryServices();

        var scope = services.BuildServiceProvider().CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<ScopedService>();
        var serviceInstanceId = service.InstanceId.ToString();

        var clientWithHandler = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("TestClient");
        var response = await clientWithHandler.GetAsync("https://example.com");
        var clientInstanceId = await response.Content.ReadAsStringAsync();

        clientInstanceId
            .Should().Be(serviceInstanceId); // This will fail, as the default handler is not scope-aware and will return a different instance ID than the scoped service.
    }

    [Fact]
    public async Task AddScopeAwareHttpHandler_ShouldReuseScopedServiceInHandler()
    {
        var services = BuildScopeAwareClientFactoryServices();

        var scope = services.BuildServiceProvider().CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<ScopedService>();
        var serviceInstanceId = service.InstanceId.ToString();

        var clientWithHandler = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("TestClient");
        var response = await clientWithHandler.GetAsync("https://example.com");
        var clientInstanceId = await response.Content.ReadAsStringAsync();

        clientInstanceId
            .Should().Be(serviceInstanceId);
    }
}
