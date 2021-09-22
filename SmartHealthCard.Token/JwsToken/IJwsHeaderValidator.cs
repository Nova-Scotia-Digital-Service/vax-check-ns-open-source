using ProofOfVaccine.Token.Support;

namespace ProofOfVaccine.Token.JwsToken
{
  public interface IJwsHeaderValidator
  {
    Result Validate<T>(T Obj);

  }
}