using DG.Common.Http.Authorization.OAuth2.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DG.Common.Http.Tests.Authorization.OAuth2
{
    internal class TestOAuthorizationServer
    {
        private readonly static TestOAuthorizationServer _instance = new TestOAuthorizationServer();
        public static TestOAuthorizationServer Instance => _instance;

        private readonly Dictionary<string, Request> _requests = new Dictionary<string, Request>();

        public void StartAuthorization(OAuthRequest request)
        {
            _requests.Add(request.State, new Request()
            {
                State = request.State,
                Scopes = request.Scopes,
                UserAccepted = false,
                CurrentRefreshToken = null
            });
        }

        public void AcceptAuthorization(OAuthRequest request, out string callbackCode)
        {
            var code = Uulsid.NewUulsid();
            _requests[request.State].UserAccepted = true;
            _requests[request.State].Code = code;
            callbackCode = code.ToString();
        }

        public void RetractAuthorization(OAuthRequest request)
        {
            _requests[request.State].UserAccepted = false;
        }

        public OAuthToken GetTokenByCallback(OAuthRequest request, string callBackCode)
        {
            var found = _requests[request.State];
            if (!Uulsid.TryParse(callBackCode, out Uulsid code))
            {
                return null;
            }
            if (found.Code != code)
            {
                return null;
            }
            found.Code = null;
            found.CurrentRefreshToken = "t." + Uulsid.NewUulsid().ToString();
            return Refresh(found.CurrentRefreshToken);
        }

        public OAuthToken Refresh(string refreshToken)
        {
            var request = _requests.FirstOrDefault(r => r.Value.CurrentRefreshToken == refreshToken).Value;
            if (request == null || !request.UserAccepted || refreshToken != request.CurrentRefreshToken)
            {
                return null;
            }

            string accessToken = "a." + Uulsid.NewUulsid().ToString();
            request.CurrentRefreshToken = "r." + Uulsid.NewUulsid().ToString();
            DateTimeOffset validUntil = DateTimeOffset.Now.AddHours(1);

            return new OAuthToken(accessToken, validUntil, request.CurrentRefreshToken);
        }

        private class Request
        {
            public string State { get; set; }
            public string[] Scopes { get; set; }
            public bool UserAccepted { get; set; }
            public Uulsid Code { get; set; }
            public string CurrentRefreshToken { get; set; }
        }
    }
}
