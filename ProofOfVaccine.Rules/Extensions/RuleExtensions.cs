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
        /// Creates a new <see cref="Rule{T}"/> from type <typeparamref name="TInput"/>.
        /// The <paramref name="input"/> is validated against the <paramref name="ruleStatement"/> function and any other rule sets appended through <see cref="AppendRule{TInput, TMessage}(RuleSet{TInput, TMessage}, Func{TInput, bool}, TMessage)"/>
        /// Each rule statement comes with it's own <paramref name="failureMessage"/>.
        /// </summary>
        /// <typeparam name="TInput">Type of input.</typeparam>
        /// <typeparam name="TMessage">Type of message returned.</typeparam>
        /// <param name="input">The input that rule statements are validated against.</param>
        /// <param name="ruleStatement">A function that valids a condition against the <paramref name="input"/>.</param>
        /// <param name="failureMessage">The message returned if the rule statement fails to validate when <see cref="RuleSet{TInput, TMessage}.Validate(TMessage)"/> is called.</param>
        /// <returns></returns>
        public static RuleSet<TInput, TMessage> CreateRuleSet<TInput, TMessage>(this TInput input, Func<TInput, bool> ruleStatement, TMessage failureMessage)
        {
            var ruleSet = new RuleSet<TInput, TMessage>(input, ruleStatement, failureMessage);
            return ruleSet;
        }

        /// <summary>
        /// Appends a rule statement to the <paramref name="ruleSet"/> created via <see cref="CreateRuleSet{TInput, TMessage}(TInput, Func{TInput, bool}, TMessage)"/>.
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <param name="ruleSet">The <see cref="RuleSet{TInput, TMessage}"/> that the rule statement is appended to.</param>
        /// <param name="ruleStatement">A function that valids a condition against the <see cref="RuleSet{TInput, TMessage}"/></param>
        /// <param name="failureMessage">The message returned if the rule statement fails to validate when <see cref="RuleSet{TInput, TMessage}.Validate(TMessage)"/> is called.</param>
        /// <returns></returns>
        public static RuleSet<TInput, TMessage> AppendRule<TInput, TMessage>(this RuleSet<TInput, TMessage> ruleSet, Func<TInput, bool> appendedRuleStatement, TMessage failureMessage)
        {
            ruleSet.Append(appendedRuleStatement, failureMessage);
            return ruleSet;
        }
    }
}
