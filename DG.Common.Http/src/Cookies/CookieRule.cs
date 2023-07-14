using DG.Common.Exceptions;
using DG.Common.Http.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;

namespace DG.Common.Http.Cookies
{
    public class CookieRule
    {
        #region Default rules list
        private static readonly CookieRule[] _standardRules = new CookieRule[]
        {
            CookieRule.ForRule("Secure cookie must have secure origin.")
                .ApplyIf(c => c.IsSecure)
                .AndCheckIf(c => c.OriginUri.IsSecure()),

            CookieRule.ForRule("Domain cannot be an IP address.")
                .ApplyIf(c => !string.IsNullOrEmpty(c.Domain))
                .AndCheckIf(c => !IPAddress.TryParse(c.Domain, out IPAddress _))
                .AndCheckIf(c => !IPAddress.TryParse(c.OriginUri.Host, out IPAddress _)),

            CookieRule.ForRule("Domain is not valid.")
                .ApplyIf(c => !string.IsNullOrEmpty(c.Domain))
                .AndCheckIf(c => c.Domain.Trim('.').Contains("."))
                .AndCheckIf(c => Uri.TryCreate("https://" + c.Domain.TrimStart('.'), UriKind.Absolute, out Uri fakeUri) && fakeUri.Host == c.Domain.TrimStart('.'))
                .AndCheckIf(c => new CookiePath(c).IsDomainMatch(c.OriginUri)),

            CookieRule.ForRule("Cookie named with __Host- prefix should adhere to host-cookie rules.")
                .ApplyIf(c => c.Name.StartsWith("__Host-", StringComparison.Ordinal))
                .AndCheckIf(c => c.IsSecure)
                .AndCheckIf(c => c.OriginUri.IsSecure())
                .AndCheckIf(c => string.IsNullOrEmpty(c.Domain))
                .AndCheckIf(c => c.Path == "/"),

            CookieRule.ForRule("Cookie named with __Secure- prefix should adhere to secure-cookie rules.")
                .ApplyIf(c => c.Name.StartsWith("__Secure-", StringComparison.Ordinal))
                .AndCheckIf(c => c.IsSecure)
                .AndCheckIf(c => c.OriginUri.IsSecure()),

            CookieRule.ForRule("Cookie with SameSite=None must also be set as Secure.")
                .ApplyIf(c => c.SameSitePolicy == SameSitePolicy.None)
                .AndCheckIf(c => c.IsSecure)
        };

        /// <summary>
        /// The default rules for cookies to be valid, according to RFC 6265.
        /// </summary>
        public static IReadOnlyList<CookieRule> StandardRules => _standardRules;
        #endregion

        private readonly string _ruleName;
        private readonly Expression<Func<IRawCookie, bool>> _ruleApplicationCheck;
        private readonly Lazy<Func<IRawCookie, bool>> _actualApplicationCheck;
        private readonly List<Func<IRawCookie, bool>> _rules;

        /// <summary>
        /// The name of this rule.
        /// </summary>
        public string Name => _ruleName;

        /// <summary>
        /// Initializes a new instance of <see cref="CookieRule"/> with the given <paramref name="ruleName"/>, <paramref name="ruleApplicationCheck"/> and <paramref name="rules"/>.
        /// </summary>
        /// <param name="ruleName"></param>
        /// <param name="ruleApplicationCheck"></param>
        /// <param name="rules"></param>
        public CookieRule(string ruleName, Expression<Func<IRawCookie, bool>> ruleApplicationCheck, List<Func<IRawCookie, bool>> rules)
        {
            ThrowIf.Parameter.IsNullOrWhiteSpace(ruleName, nameof(ruleName));
            ThrowIf.Parameter.IsNull(rules, nameof(rules));
            _ruleName = ruleName;
            _ruleApplicationCheck = ruleApplicationCheck;
            _actualApplicationCheck = new Lazy<Func<IRawCookie, bool>>(() => _ruleApplicationCheck.Compile());
            _rules = rules;
        }

        /// <summary>
        /// Sets the condition to check to see if this cookie needs to follow this rule. If <paramref name="condition"/> is <see langword="null"/> cookies are always checked.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public CookieRule ApplyIf(Expression<Func<IRawCookie, bool>> condition)
        {
            return new CookieRule(_ruleName, condition, _rules);
        }

        /// <summary>
        /// Adds a condition to check to see if this cookie adheres to this rule.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public CookieRule AndCheckIf(Func<IRawCookie, bool> condition)
        {
            ThrowIf.Parameter.IsNull(condition, nameof(condition));
            var rules = _rules.ToList();
            rules.Add(condition);
            return new CookieRule(_ruleName, _ruleApplicationCheck, rules);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CookieRule"/> with the given rule name.
        /// </summary>
        /// <param name="ruleName"></param>
        /// <returns></returns>
        public static CookieRule ForRule(string ruleName)
        {
            return new CookieRule(ruleName, null, new List<Func<IRawCookie, bool>>());
        }

        /// <summary>
        /// Returns a value indicating if the given <see cref="IRawCookie"/> adheres to this rule.
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public bool CheckCookie(IRawCookie cookie)
        {
            if (!AppliesTo(cookie))
            {
                return true;
            }
            foreach (var rule in _rules)
            {
                if (!rule(cookie))
                {
                    return false;
                }
            }
            return true;
        }

        private bool AppliesTo(IRawCookie cookie)
        {
            if (_ruleApplicationCheck == null)
            {
                return true;
            }
            return _actualApplicationCheck.Value(cookie);
        }
    }
}
