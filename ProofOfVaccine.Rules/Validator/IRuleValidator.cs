using System;
using ProofOfVaccine.Rules.Support;

namespace ProofOfVaccine.Rules.Validator
{
    public interface IRuleValidator : IDisposable
    {
        Result Validate();
    }
}
