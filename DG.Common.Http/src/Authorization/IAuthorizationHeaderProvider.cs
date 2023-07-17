namespace DG.Common.Http.Authorization
{
    /// <summary>
    /// Defines a way to retrieve values for the <c>Authorization</c> header for HTTP requests.
    /// </summary>
    public interface IAuthorizationHeaderProvider
    {
        /// <summary>
        /// Indicates if an <c>Authorization</c> header value can be provided.
        /// </summary>
        bool IsAuthorized { get; }

        /// <summary>
        /// Returns a value to be used as the <c>Authorization</c> header.
        /// </summary>
        /// <returns></returns>
        string GetAuthorizationHeaderValue();
    }
}
