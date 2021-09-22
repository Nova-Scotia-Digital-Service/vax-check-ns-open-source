using ProofOfVaccine.Token.Support;
using System.Threading.Tasks;

namespace ProofOfVaccine.Token.JwsToken
{
  public interface IJwsEncoder
  {
    Task<Result<string>> EncodeAsync<HeaderType, PayloadType>(HeaderType Header, PayloadType Payload);
  }
}