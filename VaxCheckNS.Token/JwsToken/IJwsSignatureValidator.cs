using VaxCheckNS.Token.Algorithms;
using VaxCheckNS.Token.Support;

namespace VaxCheckNS.Token.JwsToken
{
  public interface IJwsSignatureValidator
  {
    Result Validate(IAlgorithm Algorithm, string Token);

  }
}