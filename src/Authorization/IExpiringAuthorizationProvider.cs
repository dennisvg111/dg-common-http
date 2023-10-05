namespace DG.Common.Http.Authorization
{
    public interface IExpiringAuthorizationProvider
    {
        bool TryRefreshAuthorization(out ExpiringAuthorization authorization);
    }
}
