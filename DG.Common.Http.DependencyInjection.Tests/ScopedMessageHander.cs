namespace DG.Common.Http.DependencyInjection.Tests;

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