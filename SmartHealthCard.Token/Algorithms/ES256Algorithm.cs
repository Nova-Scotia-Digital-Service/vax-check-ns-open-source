using ProofOfVaccine.Token.Encoders;
using ProofOfVaccine.Token.Model.Jwks;
using ProofOfVaccine.Token.Serializers.Json;
using ProofOfVaccine.Token.Support;
using System;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Asn1;
using System.Linq;

namespace ProofOfVaccine.Token.Algorithms
{
  public sealed class ES256Algorithm : IAlgorithm 
  {

    private ISigner Verifier;

    public ES256Algorithm(ISigner Verifier)
    {
      this.Verifier = Verifier ?? throw new ArgumentNullException();
    }

    public static string Name => "ES256";
    public static string KeyTypeName => "EC";
    public static string CurveName => "P-256";

    public Result<bool> Verify(byte[] bytesToSign, byte[] signature)
    {
      if (Verifier is null)
        return Result<bool>.Fail("Verifier has not been initialized.");

      Verifier.BlockUpdate(bytesToSign, 0, bytesToSign.Length);
      var derSignature = new DerSequence(
        // first 32 bytes is "r" number
        new DerInteger(new BigInteger(1, signature.Take(32).ToArray())),
        // last 32 bytes is "s" number
        new DerInteger(new BigInteger(1, signature.Skip(32).ToArray())))
            .GetDerEncoded();

      if (Verifier.VerifySignature(derSignature))
      {
        return Result<bool>.Ok(true);      
      }
      return Result<bool>.Fail("Failed to verify the signature.");
    }

    public static Result<IAlgorithm> FromJWKS(string Kid, JsonWebKeySet JsonWebKeySet)
    {
      JsonWebKey? Key = JsonWebKeySet.Keys.Find(x => x.Kid.Equals(Kid, StringComparison.CurrentCulture));
      if (Key is null)
        return Result<IAlgorithm>.Fail($"No key matching the token's header kid value of: {Kid} could be found in the sourced Jason Web Key Set (JWKS) file.");

      // TODO: Add support for Trust. Focusing on White-List approach for now.
      try
      {
        var ecParams = NistNamedCurves.GetByName(CurveName);
        var domainParameters = new ECDomainParameters(ecParams.Curve, ecParams.G, ecParams.N, ecParams.H, ecParams.GetSeed());
        var curve = ecParams.Curve;
        var x = new BigInteger(1, Base64UrlEncoder.Decode(Key.X));
        var y = new BigInteger(1, Base64UrlEncoder.Decode(Key.Y));
        var q = curve.CreatePoint(x, y);

        var pubkeyParam = new ECPublicKeyParameters(q, domainParameters);
        var Verifier = SignerUtilities.GetSigner("SHA-256withECDSA");
        Verifier.Init(false, pubkeyParam);

        return Result<IAlgorithm>.Ok(new ES256Algorithm(Verifier));
      }
      catch (Exception)
      {
        return Result<IAlgorithm>.Fail("Could not create signer used to verify signature using JWKS.");
      }
    }
  }
}
