using ProofOfVaccine.Token.Support;

namespace ProofOfVaccine.Token.JwsToken
{
  public interface IJwsPayloadValidator
  {
    Result Validate<T>(T Obj);
  }
}