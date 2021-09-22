﻿using ProofOfVaccine.Token.Algorithms;
using ProofOfVaccine.Token.Exceptions;
using ProofOfVaccine.Token.JwsToken;
using ProofOfVaccine.Token.Model.Jwks;
using ProofOfVaccine.Token.Model.Shc;
using ProofOfVaccine.Token.Providers;
using ProofOfVaccine.Token.Serializers.Json;
using ProofOfVaccine.Token.Serializers.Jws;
using ProofOfVaccine.Token.Serializers.Shc;
using ProofOfVaccine.Token.Support;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ProofOfVaccine.Token
{
  /// <summary>
  /// A SMART Health Card decoder. 
  /// Take a SMART Health Card JWS token and decode's its payload to either a JSON string or an object model
  /// Optionally verify the token's signing signature.
  /// For production systems it is highly advised that you do verify the signature. 
  /// </summary>
  public class SmartHealthCardDecoder
  {
    private readonly IJsonSerializer JsonSerializer;
    private readonly IJwsHeaderSerializer HeaderSerializer;
    private readonly IJwsPayloadSerializer PayloadSerializer;

    private IJwksProvider? JwksProvider;
    private IHttpClient? HttpClient;
    private IJwsSignatureValidator? JwsSignatureValidator;
    private IJwsHeaderValidator? JwsHeaderValidator;
    private IJwsPayloadValidator? JwsPayloadValidator;

    /// <summary>
    /// Default Constructor
    /// </summary>
    public SmartHealthCardDecoder()
    {
      this.JsonSerializer = new JsonSerializer();
      this.HeaderSerializer = new SmartHealthCardJwsHeaderSerializer(JsonSerializer);
      this.PayloadSerializer = new SmartHealthCardJwsPayloadSerializer(JsonSerializer);
    }

    /// <summary>
    /// Provide any implementation of the IJwksProvider interface to override the default implementation
    /// This allows you to inject a JWKS file to be used when validating the JWS signature instead of the 
    /// default implementation that will attempt to source the JWKS from the token's Issuer URL (iss) + /.well-known/jwks.json 
    /// </summary>
    /// <param name="JwksProvider">Provides the provider for sourcing the JWKS file for token signature verifying</param>
    public SmartHealthCardDecoder(IJwksProvider JwksProvider)
      :this()
    {
      this.JwksProvider = JwksProvider;
    }

    /// <summary>
    /// Provide any implementation of the IJwksProvider interface to override the default implementation
    /// This allows you to inject a JWKS file to be used when validating the JWS signature instead of the 
    /// default implementation that will attempt to source the JWKS from the token's Issuer URL (iss) + /.well-known/jwks.json 
    /// </summary>
    /// <param name="JwksProvider">Provides the provider for sourcing the JWKS file for token signature verifying</param>
    public SmartHealthCardDecoder(IHttpClient HttpClient)
      : this()
    {
      this.HttpClient = HttpClient;
    }

    /// <summary>
    /// Provide any implementation of the following interfaces to override their default implementation
    /// </summary>
    /// <param name="JsonSerializer">Provides an implementation of a basic JSON serialization</param>
    /// <param name="HeaderSerializer">Provides an implementation that performs the serialization of the data that is packed into the JWS Header</param>
    /// <param name="PayloadSerializer">Provides an implementation that performs the serialization of the data that is packed into the JWS Payload</param>
    public SmartHealthCardDecoder(IJsonSerializer? JsonSerializer, IJwsHeaderSerializer? HeaderSerializer, IJwsPayloadSerializer? PayloadSerializer)
    {
      this.JsonSerializer = JsonSerializer ?? new JsonSerializer();
      this.HeaderSerializer = HeaderSerializer ?? new SmartHealthCardJwsHeaderSerializer(this.JsonSerializer);
      this.PayloadSerializer = PayloadSerializer ?? new SmartHealthCardJwsPayloadSerializer(this.JsonSerializer);
    }

    /// <summary>
    /// Provide any implementation of the following interfaces to override their default implementation 
    /// </summary>
    /// <param name="JsonSerializer">Provides an implementation of a basic JSON serialization</param>
    /// <param name="HeaderSerializer">Provides an implementation that performs the serialization of the data that is packed into the JWS Header</param>
    /// <param name="PayloadSerializer">Provides an implementation that performs the serialization of the data that is packed into the JWS Payload</param>
    /// <param name="JwksProvider">Provides an implementation that sources the JWKS file for token signature verifying</param>
    /// <param name="JwsSignatureValidator">Provides an implementation of the JWS signature verifying</param>
    /// <param name="JwsHeaderValidator">Provides an implementation that performs the serialization of the data that is packed into the JWS Payload</param>
    /// <param name="JwsPayloadValidator">Provides an implementation that performs the serialization of the data that is packed into the JWS Payload</param>
    public SmartHealthCardDecoder(IJsonSerializer? JsonSerializer, IJwsHeaderSerializer? HeaderSerializer, IJwsPayloadSerializer? PayloadSerializer,
      IJwksProvider? JwksProvider, IJwsSignatureValidator? JwsSignatureValidator, IJwsHeaderValidator? JwsHeaderValidator, IJwsPayloadValidator? JwsPayloadValidator)
    {
      this.JsonSerializer = JsonSerializer ?? new JsonSerializer();
      this.HeaderSerializer = HeaderSerializer ?? new SmartHealthCardJwsHeaderSerializer(this.JsonSerializer);
      this.PayloadSerializer = PayloadSerializer ?? new SmartHealthCardJwsPayloadSerializer(this.JsonSerializer);

      this.JwksProvider = JwksProvider;
      this.JwsSignatureValidator = JwsSignatureValidator;
      this.JwsHeaderValidator = JwsHeaderValidator;
      this.JwsPayloadValidator = JwsPayloadValidator;
    }

    /// <summary>
    /// Decode a SMART Health Card JWS Token to its JSON form of the SMART Health Card verifiable credentials
    /// </summary>
    /// <param name="Token"></param>
    /// <param name="Verify"></param>
    /// <returns></returns>
    public async Task<string> DecodeToJsonAsync(string Token, bool Verify = true)
    {
      SmartHealthCardModel SmartHealthCardModel = await DecodeAsync(Token, Verify);
      Result<string> ToJsonResult = JsonSerializer.ToJson(SmartHealthCardModel, Minified: false);
      if (ToJsonResult.Failure)
        throw new SmartHealthCardDecoderException(ToJsonResult.Message);

      return ToJsonResult.Value;
    }

    /// <summary>
    /// Decode a SMART Health Card JWS Token to a object model form of the SMART Health Card verifiable credentials
    /// </summary>
    /// <param name="Token"></param>
    /// <param name="Verify"></param>
    /// <returns></returns>
    public async Task<SmartHealthCardModel> DecodeAsync(string Token, bool Verify = true)
    {
      if (Verify)
      {
        IJwsDecoder JwsDecoder = new SmartHealthCardJwsDecoder(
          this.JsonSerializer,
          this.HeaderSerializer,
          this.PayloadSerializer,
          this.JwksProvider,
          this.HttpClient,
          this.JwsSignatureValidator,
          this.JwsHeaderValidator,
          this.JwsPayloadValidator);
        Result<SmartHealthCardModel> DecodePayloadResult = await JwsDecoder.DecodePayloadAsync<SmartHealthCareJWSHeaderModel, SmartHealthCardModel>(Token: Token, Verity: Verify);
        if (DecodePayloadResult.Failure)
          throw new SmartHealthCardDecoderException(DecodePayloadResult.Message);

        return DecodePayloadResult.Value;
      }
      else
      {
        IJwsDecoder JwsDecoder = new SmartHealthCardJwsDecoder(
          this.HeaderSerializer,
          this.PayloadSerializer);

        Result<SmartHealthCardModel> DecodePayloadResult = await JwsDecoder.DecodePayloadAsync<SmartHealthCareJWSHeaderModel, SmartHealthCardModel>(Token: Token, Verity: Verify);
        if (DecodePayloadResult.Failure)
          throw new SmartHealthCardDecoderException(DecodePayloadResult.Message);

        return DecodePayloadResult.Value;
      }
    }
  }
}
