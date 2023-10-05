using DG.Common.Http.Extensions;
using System;
using System.Collections.Generic;
using System.Net;

namespace DG.Common.Http.Cookies
{
    /// <summary>
    /// A collection of rules to check the validity of a cookie.
    /// </summary>
    public static class CookieRules
    {
        private static readonly ValidityRule[] _standardRules = new ValidityRule[]
        {
            ValidityRule.WithName("Secure cookie must have secure origin.")
                .ApplyIf(c => c.IsSecure)
                .AndCheckIf(c => c.OriginUri.IsSecure()),

            ValidityRule.WithName("Domain cannot be an IP address.")
                .ApplyIf(c => !string.IsNullOrEmpty(c.Domain))
                .AndCheckIf(c => !IPAddress.TryParse(c.Domain, out IPAddress _))
                .AndCheckIf(c => !IPAddress.TryParse(c.OriginUri.Host, out IPAddress _)),

            ValidityRule.WithName("Domain is not valid.")
                .ApplyIf(c => !string.IsNullOrEmpty(c.Domain))
                .AndCheckIf(c => c.Domain.Trim('.').Contains("."))
                .AndCheckIf(c => Uri.TryCreate("https://" + c.Domain.TrimStart('.'), UriKind.Absolute, out Uri fakeUri) && fakeUri.Host == c.Domain.TrimStart('.'))
                .AndCheckIf(c => new CookiePath(c).IsDomainMatch(c.OriginUri)),

            ValidityRule.WithName("Cookie named with __Host- prefix should adhere to host-cookie rules.")
                .ApplyIfCookieNameStartsWith("__Host-")
                .AndCheckIf(c => c.IsSecure)
                .AndCheckIf(c => c.OriginUri.IsSecure())
                .AndCheckIf(c => string.IsNullOrEmpty(c.Domain))
                .AndCheckIf(c => c.Path == "/"),

            ValidityRule.WithName("Cookie named with __Secure- prefix should adhere to secure-cookie rules.")
                .ApplyIfCookieNameStartsWith("__Secure-")
                .AndCheckIf(c => c.IsSecure)
                .AndCheckIf(c => c.OriginUri.IsSecure()),

            ValidityRule.WithName("Cookie with SameSite=None must also be set as Secure.")
                .ApplyIf(c => c.SameSitePolicy == SameSitePolicy.None)
                .AndCheckIf(c => c.IsSecure)
        };


        /// <summary>
        /// The default rules for cookies to be valid, according to RFC 6265.
        /// </summary>
        public static IReadOnlyList<ValidityRule> Default => _standardRules;
    }
}
