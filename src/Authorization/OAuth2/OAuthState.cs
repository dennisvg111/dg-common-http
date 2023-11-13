namespace DG.Common.Http.Authorization.OAuth2
{
    /// <summary>
    /// The default algorithm to generate new state values, for authorization requests.
    /// </summary>
    public static class OAuthState
    {
        /// <summary>
        /// Returns a new state value.
        /// </summary>
        /// <returns></returns>
        public static string NewState()
        {
            return Uulsid.NewUulsid().ToString();
        }
    }
}
