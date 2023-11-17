# dg-common-http
 Web access utilities for c#

- [Fluent HTTP requests](#fluent-http-requests)
- [OAuth authorization](#oauth-authorization)
  * [Implementing IOAuthLogic](#implementing-ioauthlogic)
  * [Starting an authorization flow](#starting-an-authorization-flow)

## Fluent HTTP requests
Using the FluentRequest class, which seamlessly integrates in .Net's System.Net.Http environment, it is possible to easily set up a web request by chaining method calls.
This allows you to replace the following code

```cs
using System.Net.Http;

var client = new HttpClient();

var formValues = new KeyValuePair<string, string>[]
{
  new KeyValuePair<string, string>("username", "test"),
  new KeyValuePair<string, string>("password", "MyPassword123#")
};
var request = new HttpRequestMessage(HttpMethod.Post, "https://www.example.com/login")
{
  Content = new FormUrlEncodedContent(formValues)
};
request.Headers.Add("Authorization", "Bearer " + token);

var response = await client.SendAsync(request);

```
With this

```cs
using DG.Common.Http.Fluent;
using System.Net.Http;

var client = new HttpClient();

var request = FluentRequest.Post.To("https://www.example.com/login")
  .WithContent(FluentFormContentBuilder
    .With("username", "test")
    .AndWith("password", "MyPassword123#"))
  .WithAuthorizaton(FluentAuthorization.FromBearer(token));

var response = await client.SendAsync(request);
```

## OAuth authorization

### Implementing IOAuthLogic
To start using OAuth2 authorization, you first need to implement the four methods in the IOAuthLogic interface (found in the DG.Common.Http.Authorization.OAuth2.Interfaces namespace):

1. `BuildAuthorizationUri(OAuthRequest request)`

   This method should create an authorization url to redirect a user to from an authorization request with the given scopes, callback url, and state.
   for example:

   ```
   https://www.api-service.com/authorize?state=abcdefg&scopes=user.read%2Cuser.write&callback-url=https%3A%2F%2Fwww.my-own-app.com%2Fcallback
   ```

2. `GetAccessTokenAsync(OAuthRequest request, string callBackCode)`
   
   This method should call the external api with a given callBack code, and should return an access token, an expiration date for this access token, and a refresh token (if applicable).

3. `TryRefreshTokenAsync(string refreshToken)`
   
   This method should call the external api with a refresh token, and return a new access token, a new expiration date for this access token, and a new refresh token (if needed).

4. `GetHeaderForToken(string accessToken)`
   
   This method should create an authorization header value for the given access token.

For ease of use, it is recommended to implement IOAuthLogic in a class that has a public, parameterless constructor.

### Starting an authorization flow
Starting an authorization flow can be done using any class that implements IOAuthLogic, like the below example.
```cs
var scopes = new string[] { "user.read", "email.read", "email.send" };
Uri callbackUri = new Uri("https://www.my-own-app.com/callback");

//ExampleOAuthLogic must implement IOAuthLogic, and expose a parameterless constructor
var flow = OAuthFlow.StartNew<ExampleOAuthLogic>(scopes, callbackUri);

//alternatively, if ExampleOAuthLogic does not have a parameterless constructor
var logic = new ExampleOAuthLogic(clientId, clientSecret);
var flow = OAuthFlow.StartNewFor(logic, scopes, callbackUri);
```

After receiving the callback code from your callback endpoint, you can continue the authorization flow like this:
```cs
await flow.AuthorizationCallbackAsync(callbackCode);
```

This instance of OAuthFlow is now authorized, and you can now use it to create an authorization header.
The OAuthFlow class will automatically refresh the token, if needed (and possible).
```cs
var headerValue = await flow.GetAuthorizationHeaderAsync();

var request = new HttpRequestMessage(HttpMethod.Post, "https://api-service.com");
request.Headers.Add("Authorization", headerValue.ToString());

// OAuthFlow can also be used to provide Authorization headers to a FluentRequest directly, like this:
var request = FluentRequest.Post.To("https://api-service.com")
  .WithAuthorization(flow);
```

Note that an authorization flow can be interrupted and continued at any time, by simply exporting the flow and using the exported data to create a new instance.
```cs
var data = flow.Export();

OAuthFlow continued = OAuthFlow.Continue<ExampleOAuthLogic>(data);
```
