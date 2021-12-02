using Newtonsoft.Json;
using VaxCheckNS.Token.Model.Jwks;
using System;

namespace VaxCheckNS.Token.Providers
{
  public class JwksCacheItem
  {
    public JwksCacheItem(Uri wellKnownUrl, JsonWebKeySet jsonWebKeySet, DateTime obtainedDate)
    {
      WellKnownUrl = wellKnownUrl;
      JsonWebKeySet = jsonWebKeySet;
      ObtainedDate = obtainedDate;
    }

    [JsonProperty("well_known_url", Required = Required.Always)]
    public Uri WellKnownUrl { get; set; }
    [JsonProperty("jwks", Required = Required.Always)]
    public JsonWebKeySet JsonWebKeySet { get; set; }
    [JsonProperty("obtained_date", Required = Required.Always)]
    public DateTime ObtainedDate { get; set; }
  }
}
