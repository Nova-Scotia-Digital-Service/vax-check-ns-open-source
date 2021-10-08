using System;
using VaxCheckNS.Rules.Support;

namespace VaxCheckNS.Rules.Validator
{
    public interface IRuleValidator<T> : IDisposable
    {
        Result<T> Validate();
    }
}
