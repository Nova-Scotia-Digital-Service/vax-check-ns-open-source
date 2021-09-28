using System;
using ProofOfVaccine.Rules.Support;

namespace ProofOfVaccine.Rules.Rule
{
    public interface IRule : IDisposable
    {
        Result Validate();
    }
}
