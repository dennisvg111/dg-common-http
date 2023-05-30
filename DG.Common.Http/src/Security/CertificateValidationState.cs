namespace DG.Common.Http.Security
{
    /// <summary>
    /// Indicates if server certificate validation is enabled.
    /// </summary>
    public enum CertificateValidationState
    {
        /// <summary>
        /// Server certificate validation is enabled, so certificates will be checked.
        /// </summary>
        Enabled = 0,

        /// <summary>
        /// Server certificate validation is disabled, so invalid certificates will be allowed.
        /// </summary>
        Disabled = 1
    }
}
