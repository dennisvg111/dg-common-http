namespace DG.Common.Http.DependencyInjection.Tests.Helpers;

/// <summary>
/// A service that contains a unique value for <see cref="InstanceId"/> each time it is constructed.
/// </summary>
public class ScopedService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
}