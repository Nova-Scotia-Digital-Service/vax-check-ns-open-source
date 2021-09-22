using ProofOfVaccine.Token.Model.Jwks;
using ProofOfVaccine.Token.Support;
using System;
using System.Threading.Tasks;

namespace ProofOfVaccine.Token.Providers
{
  public interface IJwksProvider
  {
    Task<Result<JsonWebKeySet>> GetJwksAsync(Uri WellKnownJwksUri, System.Threading.CancellationToken? CancellationToken = null);
  }
}