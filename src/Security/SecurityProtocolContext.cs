using System;
using System.Net;
using System.Net.Security;

namespace DG.Common.Http.Security
{
    /// <summary>
    /// This class allows for easily managing <see cref="ServicePointManager"/> settings.
    /// </summary>
    public class SecurityProtocolContext : IDisposable
    {
        /// <summary>
        /// A certificate validation callback that always returns true.
        /// </summary>
        public static readonly RemoteCertificateValidationCallback AcceptAnyCertificateCallback = (s, c, chain, e) => true;

        private readonly SecurityProtocolType _oldSecurity;
        private readonly RemoteCertificateValidationCallback _oldCallback;

        /// <summary>
        /// Initializes a new instance of <see cref="SecurityProtocolContext"/> with the given accepted protocols, and validation state.
        /// </summary>
        /// <param name="acceptedProtocols"></param>
        /// <param name="validationState"></param>
        public SecurityProtocolContext(SecurityProtocolType acceptedProtocols, CertificateValidationState validationState)
        {
            _oldSecurity = ServicePointManager.SecurityProtocol;
            _oldCallback = ServicePointManager.ServerCertificateValidationCallback;

            ServicePointManager.SecurityProtocol = acceptedProtocols;
            ServicePointManager.ServerCertificateValidationCallback = validationState == CertificateValidationState.Disabled ? AcceptAnyCertificateCallback : null;
        }

        /// <summary>
        /// Creates a new unsafe context that allows any security protocol. Server certificate validation is disabled.
        /// </summary>
        public static SecurityProtocolContext UnsafeContext => CreateUnsafeContext(SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12);

        /// <summary>
        /// Creates a new context that allows the given <see cref="SecurityProtocolType"/>. Server certificate validation is enabled.
        /// </summary>
        /// <param name="acceptedProtocols"></param>
        /// <returns></returns>
        public static SecurityProtocolContext CreateContext(SecurityProtocolType acceptedProtocols)
        {
            return new SecurityProtocolContext(acceptedProtocols, CertificateValidationState.Enabled);
        }

        /// <summary>
        /// Creates a new context that allows the given <see cref="SecurityProtocolType"/>. Server certificate validation is disabled.
        /// </summary>
        /// <param name="acceptedProtocols"></param>
        /// <returns></returns>
        public static SecurityProtocolContext CreateUnsafeContext(SecurityProtocolType acceptedProtocols)
        {
            return new SecurityProtocolContext(acceptedProtocols, CertificateValidationState.Disabled);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        /// <summary>
        /// Resets the <see cref="ServicePointManager.SecurityProtocol"/> settings back to their old values.
        /// </summary>
        public void Dispose()
        {
            if (disposedValue)
            {
                return;
            }
            ServicePointManager.SecurityProtocol = _oldSecurity;
            ServicePointManager.ServerCertificateValidationCallback = _oldCallback;
            disposedValue = true;
        }
        #endregion
    }
}
