using VaxCheckNS.Token.Model.Jwks;
using VaxCheckNS.Token.Support;
using System;
using System.Collections.Generic;

namespace VaxCheckNS.Token.Providers
{
  public interface IJwksCache
  {
    Result<JsonWebKeySet> Get(Uri WellKnownUrl);
    void Set(Uri WellKnownUrl, JsonWebKeySet JsonWebKeySet);
    IList<Result<JsonWebKeySet>> ClearStale(IList<Uri> WellKnownUrls);
  }
}