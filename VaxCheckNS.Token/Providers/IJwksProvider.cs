using VaxCheckNS.Token.Model.Jwks;
using VaxCheckNS.Token.Support;
using System;
using System.Threading.Tasks;

namespace VaxCheckNS.Token.Providers
{
  public interface IJwksProvider
  {
    Task<Result<JsonWebKeySet>> GetJwksAsync(Uri WellKnownJwksUri, System.Threading.CancellationToken? CancellationToken = null);
  }
}