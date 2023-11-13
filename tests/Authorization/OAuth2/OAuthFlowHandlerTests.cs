using DG.Common.Http.Authorization.OAuth2;
using DG.Common.Http.Authorization.OAuth2.Data;
using DG.Common.Http.Authorization.OAuth2.Interfaces;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace DG.Common.Http.Tests.Authorization.OAuth2
{
    public class OAuthFlowHandlerTests
    {
        [Fact]
        public async Task AuthenticationCallback_Calls_Save()
        {
            var logic = TestOAuthLogic.New();
            var repository = Substitute.For<IOAuthRepository>();
            var handler = new OAuthFlowHandler(logic, repository);

            var flow = handler.StartNewFlow(new string[0], new System.Uri("https://www.test.com"));
            repository.Received(1).Save(Arg.Any<OAuthData>());

            var request = flow.Export();
            TestOAuthorizationServer.Instance.StartAuthorization(request);
            TestOAuthorizationServer.Instance.AcceptAuthorization(request, out string code);

            await flow.AuthenticationCallback(code);

            repository.Received(2).Save(Arg.Any<OAuthData>());
        }
    }
}
