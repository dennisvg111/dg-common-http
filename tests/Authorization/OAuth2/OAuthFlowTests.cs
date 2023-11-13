using DG.Common.Http.Authorization;
using DG.Common.Http.Authorization.OAuth2;
using DG.Common.Http.Authorization.OAuth2.Data;
using DG.Common.Http.Authorization.OAuth2.Exceptions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DG.Common.Http.Tests.Authorization.OAuth2
{
    public class OAuthFlowTests
    {
        [Fact]
        public async Task GetAuthorizationHeaderAsync_NewStarted_Throws()
        {
            var logic = TestOAuthLogic.New();
            var data = new OAuthData()
            {
                State = Guid.NewGuid().ToString(),
                Scopes = new string[0],
                CallBackUri = new System.Uri("https://www.test.com"),
                Started = DateTimeOffset.UtcNow
            };
            var flow = new OAuthFlow(logic, data);

            Func<Task<AuthorizationHeaderValue>> action = async () => await flow.GetAuthorizationHeaderAsync();

            await Assert.ThrowsAnyAsync<OAuthRequestNotCompletedException>(action);
        }

        [Fact]
        public async Task GetAuthorizationHeaderAsync_AuthorizationStopped_Throws()
        {
            var logic = TestOAuthLogic.New();
            var data = new OAuthData()
            {
                State = Guid.NewGuid().ToString(),
                Scopes = new string[0],
                CallBackUri = new System.Uri("https://www.test.com"),
                Started = DateTimeOffset.UtcNow,
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
                ValidUntill = DateTimeOffset.UtcNow.AddHours(-1)
            };
            var flow = new OAuthFlow(logic, data);

            Func<Task<AuthorizationHeaderValue>> action = async () => await flow.GetAuthorizationHeaderAsync();

            await Assert.ThrowsAnyAsync<OAuthAuthorizationExpiredException>(action);
        }
    }
}
