using System;
using ProofOfVaccine.Rules.Rule;

namespace ProofOfVaccine.Rules.Extensions
{
    /// <summary>
    /// Extension methods for creating a <see cref="Rule{T}"/>.
    /// </summary>
    public static class RuleExtensions
    {
        /// <summary>
        /// Creates a new <see cref="Rule{T}"/> from type <typeparamref name="T"/>.
        /// The <paramref name="input"/> is validated against the <paramref name="ruleStatement"/> function and any other rule sets appended through <see cref="AppendRule{T}(Rule{T}, Func{T, bool}, string)"/>
        /// Each rule statement comes with it's own <paramref name="failureMessage"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The input that rule statements are validated against.</param>
        /// <param name="ruleStatement">A function that valids a condition against the <paramref name="input"/>.</param>
        /// <param name="failureMessage">The message returned if the rule statement fails to validate when <see cref="Rule{T}.Validate"/> is called.</param>
        /// <returns></returns>
        public static Rule<T> CreateRule<T>(this T input, Func<T, bool> ruleStatement, string failureMessage)
        {
            var rule = new Rule<T>(input, ruleStatement, failureMessage);
            return rule;
        }

        /// <summary>
        /// Appends a rule statement to the <paramref name="rule"/> created via <see cref="CreateRule{T}(T, Func{T, bool}, string)"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rule">The <see cref="Rule{T}"/> that the rule statement is appended to.</param>
        /// <param name="ruleStatement">A function that valids a condition against the <see cref="Rule{T}.Input"/></param>
        /// <param name="failureMessage">The message returned if the rule statement fails to validate when <see cref="Rule{T}.Validate"/> is called.</param>
        /// <returns></returns>
        public static Rule<T> AppendRule<T>(this Rule<T> rule, Func<T, bool> appendedRuleStatement, string failureMessage)
        {
            rule.Append(appendedRuleStatement, failureMessage);
            return rule;
        }
    }
}
