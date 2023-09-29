namespace DG.Common.Http.Authorization.OAuth
{
    public interface IExpiringAuthorizationProvider
    {
        bool TryRefreshAuthorization(out ExpiringAuthorization authorization);
    }
}
