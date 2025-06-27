namespace DG.Common.Http.DependencyInjection.Tests.Helpers;

/// <summary>
/// A message handler that will always return string content with the instance ID of the <see cref="ScopedService"/> it was constructed with.
/// </summary>
public class ScopedMessageHander : DelegatingHandler
{
    private readonly ScopedService _service;

    public ScopedMessageHander(ScopedService service)
    {
        _service = service;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Constant across instances
        var instanceId = _service.InstanceId;

        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(_service.InstanceId.ToString())
        });
    }
}