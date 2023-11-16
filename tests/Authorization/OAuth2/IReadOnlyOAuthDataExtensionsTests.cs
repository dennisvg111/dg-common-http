using DG.Common.Http.Authorization.OAuth2.Data;
using System;
using Xunit;

namespace DG.Common.Http.Tests.Authorization.OAuth2
{
    public class IReadOnlyOAuthDataExtensionsTests
    {
        [Fact]
        public void AccessTokenNull_IsCompleted_ReturnsFalse()
        {
            var data = new OAuthData()
            {
                AccessToken = null
            };

            bool isComplete = data.IsCompleted();

            Assert.False(isComplete);
        }

        [Fact]
        public void AccessTokenEmpty_IsCompleted_ReturnsFalse()
        {
            var data = new OAuthData()
            {
                AccessToken = ""
            };

            bool isComplete = data.IsCompleted();

            Assert.False(isComplete);
        }

        [Fact]
        public void AccessTokenNotEmpty_IsCompleted_ReturnsTrue()
        {
            var data = new OAuthData()
            {
                AccessToken = "access-token"
            };

            bool isComplete = data.IsCompleted();

            Assert.True(isComplete);
        }

        [Fact]
        public void ExpirationDateNull_IsExpired_ReturnsFalse()
        {
            var data = new OAuthData()
            {
                AccessTokenExpirationDate = null
            };

            bool isExpired = data.IsExpired();

            Assert.False(isExpired);
        }

        [Fact]
        public void ExpirationDateFuture_IsExpired_ReturnsFalse()
        {
            var data = new OAuthData()
            {
                AccessTokenExpirationDate = DateTimeOffset.UtcNow.AddHours(1)
            };

            bool isExpired = data.IsExpired();

            Assert.False(isExpired);
        }

        [Fact]
        public void ExpirationDatePast_IsExpired_ReturnsTrue()
        {
            var data = new OAuthData()
            {
                AccessTokenExpirationDate = DateTimeOffset.UtcNow.AddHours(-1)
            };

            bool isExpired = data.IsExpired();

            Assert.True(isExpired);
        }

        [Fact]
        public void RefreshTokenNull_HasRefreshToken_ReturnsFalse()
        {
            var data = new OAuthData()
            {
                RefreshToken = null
            };

            bool isComplete = data.HasRefreshToken();

            Assert.False(isComplete);
        }

        [Fact]
        public void RefreshTokenEmpty_HasRefreshToken_ReturnsFalse()
        {
            var data = new OAuthData()
            {
                RefreshToken = ""
            };

            bool isComplete = data.HasRefreshToken();

            Assert.False(isComplete);
        }

        [Fact]
        public void RefreshTokenNotEmpty_HasRefreshToken_ReturnsTrue()
        {
            var data = new OAuthData()
            {
                RefreshToken = "refresh-token"
            };

            bool isComplete = data.HasRefreshToken();

            Assert.True(isComplete);
        }
    }
}
