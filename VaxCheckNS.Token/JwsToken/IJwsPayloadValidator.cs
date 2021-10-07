using VaxCheckNS.Token.Support;

namespace VaxCheckNS.Token.JwsToken
{
  public interface IJwsPayloadValidator
  {
    Result Validate<T>(T Obj);
  }
}