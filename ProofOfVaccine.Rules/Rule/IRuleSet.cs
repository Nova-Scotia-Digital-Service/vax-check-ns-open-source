using System;
using ProofOfVaccine.Rules.Support;

namespace ProofOfVaccine.Rules.Rule
{
    public interface IRuleSet<TInput, TMessage> : IDisposable
    {
        Result<TMessage> Validate(TMessage failureMessage);
    }
}
