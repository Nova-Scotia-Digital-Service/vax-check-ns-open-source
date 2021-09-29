using ProofOfVaccine.Token.Model.Jwks;
using ProofOfVaccine.Token.Support;
using System;
using System.Collections.Generic;

namespace ProofOfVaccine.Token.Providers
{
  public interface IJwksCache
  {
    Result<JsonWebKeySet> Get(Uri WellKnownUrl);
    void Set(Uri WellKnownUrl, JsonWebKeySet JsonWebKeySet);
    IList<Result<JsonWebKeySet>> ClearStale(IList<Uri> WellKnownUrls);
  }
}