using VaxCheckNS.Token.Support;
using System.Threading.Tasks;

namespace VaxCheckNS.Token.JwsToken
{
  public interface IJwsEncoder
  {
    Task<Result<string>> EncodeAsync<HeaderType, PayloadType>(HeaderType Header, PayloadType Payload);
  }
}