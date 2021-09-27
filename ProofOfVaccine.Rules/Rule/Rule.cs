using System;
using ProofOfVaccine.Rules.Support;

namespace ProofOfVaccine.Rules.Rule
{
    // TODO: There is probably a better name for this class?
    // TODO: Consider adding flow to rule so validation paths can be created more easily.
    /// <summary>
    /// A rule class that creates a set of rules that are used to validate some kind of object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Rule<T> : IRule
    {
        /// <summary>
        /// The input that was used to create the rule.
        /// </summary>
        private readonly T Input;

        /// <summary>
        /// The function called when the <see cref="Validate"/> method is called.
        /// </summary>
        private Func<T, Result> RuleSet;

        /// <summary>
        /// The constructor. Internally called by <see cref="Extensions.RuleExtensions"/> to create a new rule.
        /// </summary>
        /// <param name="input">The input that the function verifies.</param>
        /// <param name="InitialRule">The initial function.</param>
        /// <param name="failureMessage">The message that returns with the <see cref="Result"/>.</param>
        internal Rule(T input, Func<T, bool> InitialRule, string failureMessage)
        {
            Input = input;
            RuleSet = (t) =>
            {
                if (InitialRule(t))
                {
                    return Result.Ok();
                }
                return Result.Fail(failureMessage);
            };
        }

        /// <summary>
        /// Called by <see cref="Extensions.RuleExtensions"/> to append another rule function.
        /// <see cref="RuleSet"/> is set to an another function that wraps both the newly appended and current <see cref="RuleSet"/>.
        /// This wrapping function will execute the appended function only if the current <see cref="RuleSet"/> returns a success.
        /// This creates a call stack that results in all functions being called if they all return success.
        /// In the case of a failure, it will return to most recent failure (executing from top-to-bottom).
        /// </summary>
        /// <param name="appendedRuleStatement">The rule statement that is being appended to the set.</param>
        /// <param name="failureMessage">The failure message that returns with the <see cref="Result"/> if it's a failure.</param>
        internal void Append(Func<T, bool> appendedRuleStatement, string failureMessage)
        {
            var currentRule = RuleSet;
            RuleSet = t =>
            {
                var currentRuleResult = currentRule(t);
                if (currentRuleResult.Success)
                {
                    if (appendedRuleStatement(t))
                    {
                        return Result.Ok();
                    }
                    return Result.Fail(failureMessage);
                }
                return currentRuleResult;
            };
        }

        /// <summary>
        /// Called on a rule when it should be validate.
        /// If all of the functions added to the <see cref="RuleSet"/> call stack return a success,
        /// then the method return <see cref="Result.Ok(string)"/>.
        /// Otherwise a failure in the call stack will return <see cref="Result.Fail(string)"/>.
        /// The related failure message is returned with the result.
        /// </summary>
        /// <returns></returns>
        public Result Validate()
        {
            if (RuleSet == null)
            {
                return Result
                    .Fail($"No rules to validate.");
            }

            return RuleSet(Input);
        }

        public void Dispose()
        {
            RuleSet = null;
        }
    }
}
