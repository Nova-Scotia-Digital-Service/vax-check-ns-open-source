using System;
using ProofOfVaccine.Rules.Support;

namespace ProofOfVaccine.Rules.Validator
{
    public interface IRuleValidator<T> : IDisposable
    {
        Result<T> Validate();
    }
}
