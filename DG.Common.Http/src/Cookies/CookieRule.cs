using DG.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DG.Common.Http.Cookies
{
    public class CookieRule
    {
        private readonly string _ruleName;
        private readonly Expression<Func<ICookieIngredients, bool>> _ruleApplicationCheck;
        private readonly Lazy<Func<ICookieIngredients, bool>> _compiledApplicationCheck;
        private readonly List<Func<ICookieIngredients, bool>> _rules;

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
        public CookieRule(string ruleName, Expression<Func<ICookieIngredients, bool>> ruleApplicationCheck, List<Func<ICookieIngredients, bool>> rules)
        {
            ThrowIf.Parameter.IsNullOrWhiteSpace(ruleName, nameof(ruleName));
            ThrowIf.Parameter.IsNull(rules, nameof(rules));
            _ruleName = ruleName;
            _ruleApplicationCheck = ruleApplicationCheck;
            _compiledApplicationCheck = new Lazy<Func<ICookieIngredients, bool>>(() => _ruleApplicationCheck.Compile());
            _rules = rules;
        }

        /// <summary>
        /// Adds a condition to check to see if this cookie adheres to this rule.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public CookieRule AndCheckIf(Func<ICookieIngredients, bool> condition)
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
        public static CookieRuleWithoutApplicationCheck WithName(string ruleName)
        {
            return new CookieRuleWithoutApplicationCheck(ruleName);
        }

        /// <summary>
        /// Returns a value indicating if the given <see cref="ICookieIngredients"/> adheres to this rule.
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public bool Check(ICookieIngredients cookie)
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

        private bool AppliesTo(ICookieIngredients cookie)
        {
            return _compiledApplicationCheck.Value(cookie);
        }

        /// <summary>
        /// 
        /// </summary>
        public class CookieRuleWithoutApplicationCheck
        {
            private readonly string _ruleName;

            internal CookieRuleWithoutApplicationCheck(string ruleName)
            {
                _ruleName = ruleName;
            }

            /// <summary>
            /// Sets the condition to check to see if this cookie needs to follow this rule.
            /// </summary>
            /// <param name="condition"></param>
            /// <returns></returns>
            public CookieRule ApplyIf(Expression<Func<ICookieIngredients, bool>> condition)
            {
                ThrowIf.Parameter.IsNull(condition, nameof(condition));
                return new CookieRule(_ruleName, condition, new List<Func<ICookieIngredients, bool>>());
            }

            /// <summary>
            /// Indicates this rule should only be applied to cookies whose name start with the given <paramref name="prefix"/>.
            /// </summary>
            /// <param name="prefix"></param>
            /// <returns></returns>
            public CookieRule ApplyIfCookieNameStartsWith(string prefix)
            {
                ThrowIf.Parameter.IsNullOrEmpty(prefix, nameof(prefix));
                return ApplyIf(c => c.Name.StartsWith(prefix, StringComparison.Ordinal));
            }

            /// <summary>
            /// Indicates this rule should be applied to all cookies.
            /// </summary>
            /// <returns></returns>
            public CookieRule ApplyToAllCookies()
            {
                return ApplyIf(c => true);
            }
        }
    }
}
