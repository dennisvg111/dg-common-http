using DG.Common.Http.Authorization;
using DG.Common.Http.Authorization.OAuth2;
using DG.Common.Http.Authorization.OAuth2.Data;
using DG.Common.Http.Authorization.OAuth2.Exceptions;
using DG.Common.Http.Authorization.OAuth2.Interfaces;
using DG.Common.Threading;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DG.Common.Http.Tests.Authorization.OAuth2
{
    public class OAuthFlowTests
    {
        [Fact]
        public async Task IsAuthorizedAsync_NotCompleted_ReturnsFalse()
        {
            var logic = Substitute.For<IOAuthLogic>();
            var data = new OAuthData()
            {
                State = Guid.NewGuid().ToString(),
                Scopes = new string[0],
                CallBackUri = new System.Uri("https://www.test.com"),
                Started = DateTimeOffset.UtcNow
            };
            var flow = new OAuthFlow(logic, data);

            var isAuthorized = await flow.IsAuthorizedAsync();

            Assert.False(isAuthorized);
        }

        [Fact]
        public async Task IsAuthorizedAsync_Valid_ReturnsFalse()
        {
            var logic = Substitute.For<IOAuthLogic>();
            var data = new OAuthData()
            {
                State = Guid.NewGuid().ToString(),
                Scopes = new string[0],
                CallBackUri = new System.Uri("https://www.test.com"),
                Started = DateTimeOffset.UtcNow,
                AccessToken = Guid.NewGuid().ToString(),
                ValidUntill = DateTimeOffset.UtcNow.AddDays(1),
                RefreshToken = Guid.NewGuid().ToString()
            };
            var flow = new OAuthFlow(logic, data);

            var isAuthorized = await flow.IsAuthorizedAsync();

            Assert.True(isAuthorized);
        }

        [Fact]
        public async Task IsAuthorizedAsync_InValid_CallsRefresh()
        {
            var logic = Substitute.For<IOAuthLogic>();
            Func<TaskResult<OAuthToken>> resultFunc = () => TaskResult.Success(new OAuthToken("access-token", DateTimeOffset.UtcNow.AddDays(1), "refresh-token"));
            logic.RefreshTokenAsync(Arg.Any<string>()).Returns(Task.FromResult(resultFunc()));
            string refreshToken = "refresh-" + Guid.NewGuid();
            var data = new OAuthData()
            {
                State = Guid.NewGuid().ToString(),
                Scopes = new string[0],
                CallBackUri = new System.Uri("https://www.test.com"),
                Started = DateTimeOffset.UtcNow,
                AccessToken = Guid.NewGuid().ToString(),
                ValidUntill = DateTimeOffset.UtcNow.AddDays(-1),
                RefreshToken = refreshToken
            };
            var flow = new OAuthFlow(logic, data);

            var isAuthorized = await flow.IsAuthorizedAsync();

            await logic.Received(1).RefreshTokenAsync(Arg.Is(refreshToken));
            Assert.True(isAuthorized);
        }

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
