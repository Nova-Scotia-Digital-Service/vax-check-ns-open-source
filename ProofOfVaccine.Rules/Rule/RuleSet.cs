using System;
using ProofOfVaccine.Rules.Support;

namespace ProofOfVaccine.Rules.Rule
{
    // TODO: Consider adding flow to rule so validation paths can be created more easily.
    /// <summary>
    /// A rule class that creates a set of rules that are used to validate some kind of object.
    /// </summary>
    /// <typeparam name="TInput">The type of the input that the rule verifies.</typeparam>
    /// /// <typeparam name="TMessage">The type of the message returned.</typeparam>
    public class RuleSet<TInput, TMessage> : IRuleSet<TInput, TMessage>
    {
        /// <summary>
        /// The input that was used to create the rule.
        /// </summary>
        private readonly TInput Input;

        /// <summary>
        /// The function called when the <see cref="Validate"/> method is called.
        /// </summary>
        private Func<TInput, Result<TMessage>> RuleSetCallStack;

        /// <summary>
        /// The constructor. Internally called by <see cref="Extensions.RuleExtensions"/> to create a new rule.
        /// </summary>
        /// <param name="input">The input that the function verifies.</param>
        /// <param name="InitialRule">The initial function.</param>
        /// <param name="failureMessage">The message that returns with the <see cref="Result{T}"/>.</param>
        internal RuleSet(TInput input, Func<TInput, bool> InitialRule, TMessage failureMessage, TMessage success = default)
        {
            Input = input;
            RuleSetCallStack = (t) =>
            {
                if (InitialRule(t))
                {
                    return Result<TMessage>.Ok(success);
                }
                return Result<TMessage>.Fail(failureMessage);
            };
        }

        /// <summary>
        /// Called by <see cref="Extensions.RuleExtensions"/> to append another rule function.
        /// <see cref="RuleSetCallStack"/> is set to an another function that wraps both the newly appended and current <see cref="RuleSetCallStack"/>.
        /// This wrapping function will execute the appended function only if the current <see cref="RuleSetCallStack"/> returns a success.
        /// This creates a call stack that results in all functions being called if they all return success.
        /// In the case of a failure, it will return to most recent failure (executing from top-to-bottom).
        /// </summary>
        /// <param name="appendedRuleStatement">The rule statement that is being appended to the set.</param>
        /// <param name="failureMessage">The failure message that returns with the <see cref="Result{T}"/> if it's a failure.</param>
        internal void Append(Func<TInput, bool> appendedRuleStatement, TMessage failureMessage, TMessage successMessage = default)
        {
            var currentRule = RuleSetCallStack;
            RuleSetCallStack = t =>
            {
                var currentRuleResult = currentRule(t);
                if (currentRuleResult.Success)
                {
                    if (appendedRuleStatement(t))
                    {
                        return Result<TMessage>.Ok(successMessage);
                    }
                    return Result<TMessage>.Fail(failureMessage);
                }
                return currentRuleResult;
            };
        }

        /// <summary>
        /// Called on a rule when it should be validate.
        /// If all of the functions added to the <see cref="RuleSetCallStack"/> call stack return a success,
        /// then the method return <see cref="Result{T}.Ok(T))"/>.
        /// Otherwise a failure in the call stack will return <see cref="Result{T}.Fail(T)"/>.
        /// The related failure message is returned with the result.
        /// </summary>
        /// <returns></returns>
        public Result<TMessage> Validate(TMessage failureMessage)
        {
            if (RuleSetCallStack == null)
            {
                return Result<TMessage>
                    .Fail(failureMessage);
            }

            return RuleSetCallStack(Input);
        }

        public void Dispose()
        {
            RuleSetCallStack = null;
        }
    }
}
