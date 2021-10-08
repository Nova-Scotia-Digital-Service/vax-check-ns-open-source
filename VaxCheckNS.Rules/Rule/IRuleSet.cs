using System;
using VaxCheckNS.Rules.Support;

namespace VaxCheckNS.Rules.Rule
{
    public interface IRuleSet<TInput, TMessage> : IDisposable
    {
        Result<TMessage> Validate(TMessage failureMessage);
    }
}
