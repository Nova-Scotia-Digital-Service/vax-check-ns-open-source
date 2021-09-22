using ProofOfVaccine.Token.Model.Jwks;
using ProofOfVaccine.Token.Support;
using System;

namespace ProofOfVaccine.Token.Providers
{
  public interface IJwksCache
  {
    Result<JsonWebKeySet> Get(Uri WellKnownUrl);
    void Set(Uri WellKnownUrl, JsonWebKeySet JsonWebKeySet);
  }
}