using DG.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DG.Common.Http.Cookies
{
    /// <summary>
    /// Represents a rule to determine if a cookie is valid or not.
    /// </summary>
    public class CookieRule
    {
        private readonly string _ruleName;
        private readonly Expression<Func<ICookie, bool>> _ruleApplicationCheck;
        private readonly Lazy<Func<ICookie, bool>> _compiledApplicationCheck;
        private readonly List<Func<ICookie, bool>> _rules;

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
        public CookieRule(string ruleName, Expression<Func<ICookie, bool>> ruleApplicationCheck, List<Func<ICookie, bool>> rules)
        {
            ThrowIf.Parameter.IsNullOrWhiteSpace(ruleName, nameof(ruleName));
            ThrowIf.Parameter.IsNull(rules, nameof(rules));
            _ruleName = ruleName;
            _ruleApplicationCheck = ruleApplicationCheck;
            _compiledApplicationCheck = new Lazy<Func<ICookie, bool>>(() => _ruleApplicationCheck.Compile());
            _rules = rules;
        }

        /// <summary>
        /// Adds a condition to check to see if this cookie adheres to this rule.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public CookieRule AndCheckIf(Func<ICookie, bool> condition)
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
        public static ICookieRuleBuilder WithName(string ruleName)
        {
            return new CookieRuleBuilder(ruleName);
        }

        /// <summary>
        /// Returns a value indicating if the given <see cref="ICookie"/> adheres to this rule.
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public bool Check(ICookie cookie)
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

        private bool AppliesTo(ICookie cookie)
        {
            return _compiledApplicationCheck.Value(cookie);
        }

        /// <summary>
        /// Defines a builder for a cookie rule, with a given rule name.
        /// </summary>
        public interface ICookieRuleBuilder
        {
            /// <summary>
            /// Indicates this rule should only be applied to cookies who match the given <paramref name="condition"/>.
            /// </summary>
            /// <param name="condition"></param>
            /// <returns></returns>
            CookieRule ApplyIf(Expression<Func<ICookie, bool>> condition);

            /// <summary>
            /// Indicates this rule should only be applied to cookies whose name start with the given <paramref name="prefix"/>.
            /// </summary>
            /// <param name="prefix"></param>
            /// <returns></returns>
            CookieRule ApplyIfCookieNameStartsWith(string prefix);

            /// <summary>
            /// Indicates this rule should be applied to all cookies.
            /// </summary>
            /// <returns></returns>
            CookieRule ApplyToAllCookies();
        }

        internal class CookieRuleBuilder : ICookieRuleBuilder
        {
            private readonly string _ruleName;

            internal CookieRuleBuilder(string ruleName)
            {
                _ruleName = ruleName;
            }

            public CookieRule ApplyIf(Expression<Func<ICookie, bool>> condition)
            {
                ThrowIf.Parameter.IsNull(condition, nameof(condition));
                return new CookieRule(_ruleName, condition, new List<Func<ICookie, bool>>());
            }

            public CookieRule ApplyIfCookieNameStartsWith(string prefix)
            {
                ThrowIf.Parameter.IsNullOrEmpty(prefix, nameof(prefix));
                return ApplyIf(c => c.Name.StartsWith(prefix, StringComparison.Ordinal));
            }

            public CookieRule ApplyToAllCookies()
            {
                return ApplyIf(c => true);
            }
        }
    }
}
