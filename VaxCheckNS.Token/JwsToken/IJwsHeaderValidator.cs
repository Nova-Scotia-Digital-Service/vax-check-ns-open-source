using VaxCheckNS.Token.Support;

namespace VaxCheckNS.Token.JwsToken
{
  public interface IJwsHeaderValidator
  {
    Result Validate<T>(T Obj);

  }
}