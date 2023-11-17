using DG.Common.Http.Authorization.OAuth2.Data;
using DG.Common.Http.Authorization.OAuth2.Exceptions;
using DG.Common.Http.Authorization.OAuth2.Interfaces;
using DG.Common.Http.Fluent;
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
        public async Task AuthorizationCallbackAsync_Calls_GetAccessTokenAsync()
        {
            string callbackCode = "callback-code";
            string accessToken = "access-token";
            string refreshToken = "refresh-token";
            var logic = Substitute.For<IOAuthLogic>();
            Func<OAuthToken> resultFunc = () => new OAuthToken(accessToken, DateTimeOffset.UtcNow.AddDays(1), refreshToken);
            logic.GetAccessTokenAsync(Arg.Any<OAuthRequest>(), Arg.Is(callbackCode)).Returns(Task.FromResult(resultFunc()));

            var flow = logic.StartNewFlow(new string[0], new Uri("https://www.test.com/callback"));
            await flow.AuthorizationCallbackAsync(callbackCode);
            var result = flow.Export();

            await logic.Received(1).GetAccessTokenAsync(Arg.Any<OAuthRequest>(), Arg.Is(callbackCode));
            Assert.Equal(accessToken, result.AccessToken);
            Assert.Equal(refreshToken, result.RefreshToken);
        }

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
            var flow = logic.ContinueFlow(data);

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
                AccessTokenExpirationDate = DateTimeOffset.UtcNow.AddDays(1),
                RefreshToken = Guid.NewGuid().ToString()
            };
            var flow = logic.ContinueFlow(data);

            var isAuthorized = await flow.IsAuthorizedAsync();

            Assert.True(isAuthorized);
        }

        [Fact]
        public async Task IsAuthorizedAsync_InValid_CallsRefresh()
        {
            var logic = Substitute.For<IOAuthLogic>();
            Func<TaskResult<OAuthToken>> resultFunc = () => TaskResult.Success(new OAuthToken("access-token", DateTimeOffset.UtcNow.AddDays(1), "refresh-token"));
            logic.TryRefreshTokenAsync(Arg.Any<string>()).Returns(Task.FromResult(resultFunc()));
            string refreshToken = "refresh-" + Guid.NewGuid();
            var data = new OAuthData()
            {
                State = Guid.NewGuid().ToString(),
                Scopes = new string[0],
                CallBackUri = new System.Uri("https://www.test.com"),
                Started = DateTimeOffset.UtcNow,
                AccessToken = Guid.NewGuid().ToString(),
                AccessTokenExpirationDate = DateTimeOffset.UtcNow.AddDays(-1),
                RefreshToken = refreshToken
            };
            var flow = logic.ContinueFlow(data);

            var isAuthorized = await flow.IsAuthorizedAsync();

            await logic.Received(1).TryRefreshTokenAsync(Arg.Is(refreshToken));
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
            var flow = logic.ContinueFlow(data);

            Func<Task<FluentAuthorization>> action = async () => await flow.GetAuthorizationHeaderAsync();

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
                AccessTokenExpirationDate = DateTimeOffset.UtcNow.AddHours(-1)
            };
            var flow = logic.ContinueFlow(data);

            Func<Task<FluentAuthorization>> action = async () => await flow.GetAuthorizationHeaderAsync();

            await Assert.ThrowsAnyAsync<OAuthAuthorizationExpiredException>(action);
        }

        [Fact]
        public void Export_ContainsAllData()
        {
            var logic = Substitute.For<IOAuthLogic>();
            OAuthData data = new OAuthData()
            {
                State = "request-state",
                Scopes = new string[] { "user-read", "mail-read", "mail-send" },
                CallBackUri = new Uri("https://www.test.com/callback"),
                Started = new DateTimeOffset(2023, 11, 16, 10, 33, 00, TimeSpan.FromHours(1)),
                AccessToken = "access-token",
                AccessTokenExpirationDate = DateTimeOffset.UtcNow.AddDays(10),
                RefreshToken = "refresh-token"
            };

            var flow = logic.ContinueFlow(data);
            var export = flow.Export();

            //export should return new instance.
            Assert.NotEqual(data, export);

            Assert.Equal(data.State, export.State);
            Assert.Equal(data.Scopes, export.Scopes);
            Assert.Equal(data.CallBackUri, export.CallBackUri);
            Assert.Equal(data.Started, export.Started);
            Assert.Equal(data.AccessToken, export.AccessToken);
            Assert.Equal(data.AccessTokenExpirationDate, export.AccessTokenExpirationDate);
            Assert.Equal(data.RefreshToken, export.RefreshToken);
        }
    }
}
