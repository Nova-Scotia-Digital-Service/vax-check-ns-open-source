using ProofOfVaccine.Token.Algorithms;
using ProofOfVaccine.Token.Encoders;
using ProofOfVaccine.Token.Exceptions;
using ProofOfVaccine.Token.Model.Jws;
using ProofOfVaccine.Token.Support;
using System;

namespace ProofOfVaccine.Token.JwsToken
{
  public sealed class JwsSignatureValidator : IJwsSignatureValidator
  {    
    public Result Validate(IAlgorithm Algorithm, string Token)
    {
      if (string.IsNullOrEmpty(Token))
        return Result.Fail("The provided Token was found to be null or empty.");
 
      Result<JwsParts> JwtPartsParseResult = JwsParts.ParseToken(Token);
      if (JwtPartsParseResult.Failure)
        return Result.Fail(JwtPartsParseResult.Message);

      byte[] BytesToSign = Utf8EncodingSupport.GetBytes(JwtPartsParseResult.Value.Header, (byte)'.', JwtPartsParseResult.Value.Payload);
      byte[] Signature = Base64UrlEncoder.Decode(JwtPartsParseResult.Value.Signature);
      Result<bool> VerifyResult = Algorithm.Verify(BytesToSign, Signature);

      if (VerifyResult.Failure)
      {
        return Result.Fail($"The JWS signing signature is invalid. {VerifyResult.Message}");       
      }
      return Result.Ok();
    }
  }
}
