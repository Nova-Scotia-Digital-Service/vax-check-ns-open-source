using ProofOfVaccine.Token.Encoders;
using ProofOfVaccine.Token.Model.Jws;
using ProofOfVaccine.Token.Providers;
using ProofOfVaccine.Token.Serializers.Json;
using ProofOfVaccine.Token.Serializers.Jws;
using ProofOfVaccine.Token.Support;
using System;
using System.Threading.Tasks;

namespace ProofOfVaccine.Token.JwsToken
{
  public sealed class JwsDecoder : IJwsDecoder
  {
    private readonly IJsonSerializer? JsonSerializer;
    private readonly IJwsHeaderSerializer HeaderSerializer;
    private readonly IJwsPayloadSerializer PayloadSerializer;
    private readonly IJwsSignatureValidator? JwsSignatureValidator;
    private readonly IJwsPayloadValidator? JwsPayloadValidator;
    private readonly IJwsHeaderValidator? JwsHeaderValidator;
    private readonly IJwksProvider? JwksProvider;
    public JwsDecoder(
      IJwsHeaderSerializer HeaderSerializer,
      IJwsPayloadSerializer PayloadSerializer)
    {
      this.HeaderSerializer = HeaderSerializer;
      this.PayloadSerializer = PayloadSerializer;
    }

    public JwsDecoder(
      IJsonSerializer JsonSerializer,
      IJwsHeaderSerializer HeaderSerializer,
      IJwsPayloadSerializer PayloadSerializer,
      IJwksProvider? JwksProvider,
      IHttpClient? HttpClient,
      IJwsSignatureValidator? IJwsSignatureValidator,
      IJwsHeaderValidator? JwsHeaderValidator,
      IJwsPayloadValidator? IJwsPayloadValidator)
      : this(HeaderSerializer, PayloadSerializer)
    {      
      this.JsonSerializer = JsonSerializer ?? new JsonSerializer();     
      this.JwksProvider = JwksProvider ?? new JwksProvider(HttpClient ?? Providers.HttpClient.Create(), this.JsonSerializer);
      this.JwsSignatureValidator = IJwsSignatureValidator;
      this.JwsHeaderValidator = JwsHeaderValidator;
      this.JwsPayloadValidator = IJwsPayloadValidator;
    }

    public async Task<Result<PayloadType>> DecodePayloadAsync<HeaderType, PayloadType>(string Token, bool Verity = false)
    {
      if (string.IsNullOrEmpty(Token))
        return await Task.FromResult(Result<PayloadType>.Fail("The provided Token was found to be null or empty."));

      throw await Task.FromResult(new NotImplementedException());

    }

    public async Task<Result<HeaderType>> DecodeHeaderAsync<HeaderType>(string Token)
    {
      if (string.IsNullOrEmpty(Token))
      {
        throw new ArgumentException(nameof(Token));
      }
      Result<JwsParts> JwsPartsParseResult = JwsParts.ParseToken(Token);
      if (JwsPartsParseResult.Failure)
        return await Task.FromResult(Result<HeaderType>.Fail(JwsPartsParseResult.Message));

      byte[] DecodedHeader = Base64UrlEncoder.Decode(JwsPartsParseResult.Value.Header);
      return await HeaderSerializer.DeserializeAsync<HeaderType>(DecodedHeader);      
    }


  }
}