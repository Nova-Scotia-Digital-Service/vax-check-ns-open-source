using ProofOfVaccine.Token.Algorithms;
using ProofOfVaccine.Token.Support;

namespace ProofOfVaccine.Token.JwsToken
{
  public interface IJwsSignatureValidator
  {
    Result Validate(IAlgorithm Algorithm, string Token);

  }
}