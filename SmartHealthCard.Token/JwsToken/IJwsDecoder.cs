using System.Threading.Tasks;
using ProofOfVaccine.Token.Support;

namespace ProofOfVaccine.Token.JwsToken
{
  public interface IJwsDecoder
  {
    Task<Result<HeaderType>> DecodeHeaderAsync<HeaderType>(string Token);
    Task<Result<PayloadType>> DecodePayloadAsync<HeaderType, PayloadType>(string Token, bool Verity = false);
  }
}