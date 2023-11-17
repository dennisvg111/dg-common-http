using DG.Common.Http.Authorization.OAuth2.Data;
using DG.Common.Http.Authorization.OAuth2.Interfaces;
using DG.Common.Http.Fluent;
using DG.Common.Threading;
using System;
using System.Threading.Tasks;

namespace DG.Common.Http.Tests.Authorization.OAuth2
{
    public class TestOAuthLogic : IOAuthLogic
    {
        public static TestOAuthLogic New()
        {
            return new TestOAuthLogic();
        }

        public Uri BuildAuthorizationUri(OAuthRequest request)
        {
            return new UriBuilder("https://authorization.com")
                .WithQuery("callback_uri", request.CallBackUri.OriginalString)
                .WithQuery("state", request.State)
                .WithQuery("scopes", string.Join(",", request.Scopes))
                .Uri;
        }

        public async Task<OAuthToken> GetAccessTokenAsync(OAuthRequest request, string callBackCode)
        {
            var token = TestOAuthorizationServer.Instance.GetTokenByCallback(request, callBackCode);
            return await Task.FromResult(token);
        }

        public FluentAuthorization GetHeaderForToken(string accessToken)
        {
            throw new NotImplementedException();
        }

        public async Task<TaskResult<OAuthToken>> TryRefreshTokenAsync(string refreshToken)
        {
            var token = TestOAuthorizationServer.Instance.Refresh(refreshToken);
            if (token != null)
            {
                return await Task.FromResult(TaskResult.Success(token));
            }
            return await Task.FromResult(TaskResult.Failure<OAuthToken>());
        }
    }
}
