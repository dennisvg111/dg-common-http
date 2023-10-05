using DG.Common.Http.Extensions;
using System.Net.Http;

namespace DG.Common.Http.Authorization
{
    /// <summary>
    /// This class provides extension methods for <see cref="IAuthorizationHeaderProvider"/>.
    /// </summary>
    public static class IAuthorizationProviderExtensions
    {
        public static void TryDecorateMessage(this IAuthorizationHeaderProvider authorizationProvider, HttpRequestMessage message)
        {
            if (authorizationProvider == null || !authorizationProvider.IsAuthorized)
            {
                return;
            }
            message.Headers.AddOrReplace("Authorization", authorizationProvider.GetAuthorizationHeaderValue());
        }
    }
}
