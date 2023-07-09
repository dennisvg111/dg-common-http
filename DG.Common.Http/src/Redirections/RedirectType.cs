namespace DG.Common.Http.Redirections
{
    /// <summary>
    /// 
    /// </summary>
    public enum RedirectType
    {
        /// <summary>
        /// Indicates that no redirection should happen.
        /// </summary>
        None,

        /// <summary>
        /// Indicates that redirection should happen with the original HTTP method.
        /// </summary>
        KeepHttpMethod,

        /// <summary>
        /// Indicates that redirection should happen as a GET request.
        /// </summary>
        ChangeToGet
    }
}
