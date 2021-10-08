using System;
using System.Collections.Generic;
using Moq;
using Org.BouncyCastle.Crypto;
using VaxCheckNS.Token.Algorithms;
using VaxCheckNS.Token.Encoders;
using VaxCheckNS.Token.Model.Jwks;
using VaxCheckNS.Token.Model.Jws;
using VaxCheckNS.Token.Support;
using Xunit;

namespace VaxCheckNS.Token.Tests.Algorithms
{
    public class ES256AlgorithmTests
    {
        private readonly Mock<ISigner> _verifier;

        private readonly string _kid = "3Kfdg-XwP-7gXyywtUfUADwBumDOPKMQx-iELL11W9s";

        private readonly string _missingKey = "3Kfdg-XwP-7gXyywtUfUADwBumDOPKMQx-iELHELLO";

        private readonly JsonWebKeySet ValidJsonWebKeySet;

        private readonly JsonWebKeySet JsonWebKeySetWithBadXCoordinate;

        private readonly JsonWebKeySet JsonWebKeySetWithMissingKey;

        private readonly string ValidCompactJWS = "eyJ6aXAiOiJERUYiLCJhbGciOiJFUzI1NiIsImtpZCI6IjNLZmRnLVh3UC03Z1h5eXd0VWZVQUR3QnVtRE9QS01ReC1pRUxMMTFXOXMifQ." +
            "3ZJLb9swEIT_SrC9ypKotjakW50CfRyKAk17CXygqbXFgg-BpIS4gf57d2kHfSDJqafqtuLw48yQ96BjhA6GlMbYVVUcUZXRypAGlCYNpZKhjxXeSTsajBWpJwxQgNsfoBPrl6Jp6_VGlOu6LWBW0N1DOo0I3e0v5t-4F-dhxQOhntZpayenf8ikvXtWqPyse9HCrgAVsEeXtDRfpv13VIktHQYdvmGIzOngVVmXgnj8dzu53iBrAkY_BYU32T5cFopLHFDeGKKdndAB4UQZiTwZ8zUYEjzs72oSPAyPgD9THNrPHUqLZ4i02hAP3jjShJjPOOoZHff40Q88b0vYLRRwryn8W5mYJdrXYlWLVVPDshSPuhHPu_nwZ8UxyTTFHJcvPCFf0CyV0g6vfZ8JyvfaHbPxeIoJ7eX90M0MZlP6cKy42SrqvlLzHQFU3glNvYFltxQwXirIdg4Y0LG33xskkVdqCnmJw95oe0Y0OXDNsaiqgw-W3iN7kSr5wMhex9HIXOf2-uodOgzSXL33cdRJGiqKSjQ-fZrsnrdCnT_xZIPNf9lg0_7rBje8sND3Ew." +
            "3AKPLpLUQuO2-N8bDOleg79w-_yQo_4FYODF2RvHKvYf6e6-09Gyi481NrBKQhiE7tig7XY7cUYdL6SzgJh2Qw";

        private readonly string CompactJWSWithModifiedPayload = "eyJ6aXAiOiJERUYiLCJhbGciOiJFUzI1NiIsImtpZCI6IjNLZmRnLVh3UC03Z1h5eXd0VWZVQUR3QnVtRE9QS01ReC1pRUxMMTFXOXMifQ." +
           "MODIFIEDEIT_SrC9ypKotjakW50CfRyKAk17CXygqbXFgg-BpIS4gf57d2kHfSDJqafqtuLw48yQ96BjhA6GlMbYVVUcUZXRypAGlCYNpZKhjxXeSTsajBWpJwxQgNsfoBPrl6Jp6_VGlOu6LWBW0N1DOo0I3e0v5t-4F-dhxQOhntZpayenf8ikvXtWqPyse9HCrgAVsEeXtDRfpv13VIktHQYdvmGIzOngVVmXgnj8dzu53iBrAkY_BYU32T5cFopLHFDeGKKdndAB4UQZiTwZ8zUYEjzs72oSPAyPgD9THNrPHUqLZ4i02hAP3jjShJjPOOoZHff40Q88b0vYLRRwryn8W5mYJdrXYlWLVVPDshSPuhHPu_nwZ8UxyTTFHJcvPCFf0CyV0g6vfZ8JyvfaHbPxeIoJ7eX90M0MZlP6cKy42SrqvlLzHQFU3glNvYFltxQwXirIdg4Y0LG33xskkVdqCnmJw95oe0Y0OXDNsaiqgw-W3iN7kSr5wMhex9HIXOf2-uodOgzSXL33cdRJGiqKSjQ-fZrsnrdCnT_xZIPNf9lg0_7rBje8sND3Ew." +
           "3AKPLpLUQuO2-N8bDOleg79w-_yQo_4FYODF2RvHKvYf6e6-09Gyi481NrBKQhiE7tig7XY7cUYdL6SzgJh2Qw";

        private readonly string CompactJWSWithInvalidSignature = "eyJ6aXAiOiJERUYiLCJhbGciOiJFUzI1NiIsImtpZCI6IjNLZmRnLVh3UC03Z1h5eXd0VWZVQUR3QnVtRE9QS01ReC1pRUxMMTFXOXMifQ." +
           "3ZJLb9swEIT_SrC9ypKotjakW50CfRyKAk17CXygqbXFgg-BpIS4gf57d2kHfSDJqafqtuLw48yQ96BjhA6GlMbYVVUcUZXRypAGlCYNpZKhjxXeSTsajBWpJwxQgNsfoBPrl6Jp6_VGlOu6LWBW0N1DOo0I3e0v5t-4F-dhxQOhntZpayenf8ikvXtWqPyse9HCrgAVsEeXtDRfpv13VIktHQYdvmGIzOngVVmXgnj8dzu53iBrAkY_BYU32T5cFopLHFDeGKKdndAB4UQZiTwZ8zUYEjzs72oSPAyPgD9THNrPHUqLZ4i02hAP3jjShJjPOOoZHff40Q88b0vYLRRwryn8W5mYJdrXYlWLVVPDshSPuhHPu_nwZ8UxyTTFHJcvPCFf0CyV0g6vfZ8JyvfaHbPxeIoJ7eX90M0MZlP6cKy42SrqvlLzHQFU3glNvYFltxQwXirIdg4Y0LG33xskkVdqCnmJw95oe0Y0OXDNsaiqgw-W3iN7kSr5wMhex9HIXOf2-uodOgzSXL33cdRJGiqKSjQ-fZrsnrdCnT_xZIPNf9lg0_7rBje8sND3Ew." +
           "MODIFIEDQuO2-N8bDOleg79w-_yQo_4FYODF2RvHKvYf6e6-09Gyi481NrBKQhiE7tig7XY7cUYdL6SzgJh2Qw";

        public ES256AlgorithmTests()
        {
            _verifier = new Mock<ISigner>();

            ValidJsonWebKeySet = new JsonWebKeySet(new List<JsonWebKey>
            {
                new JsonWebKey("EC", _kid, "sig", "ES256", "P-256", "11XvRWy1I2S0EyJlyf_bWfw_TQ5CJJNLw78bHXNxcgw", "eZXwxvO1hvCY0KucrPfKo7yAyMT6Ajc3N7OkAB6VYy8"),
            });

            JsonWebKeySetWithBadXCoordinate = new JsonWebKeySet(new List<JsonWebKey>
            {
                new JsonWebKey("EC", _kid, "sig", "ES256", "P-256", "11XvRWy1I2S0EyJlyf_bWfw_TQ5CJJJJJJNLw78bHXNxcgw", "eZXwxvO1hvCY0KucrPfKo7yAyMT6Ajc3N7OkAB6VYy8"),
            });

            JsonWebKeySetWithMissingKey = new JsonWebKeySet(new List<JsonWebKey>
            {
                new JsonWebKey("EC", _missingKey, "sig", "ES256", "P-256", "11XvRWy1I2S0EyJlyf_bWfw_TQ5CJJJJJJNLw78bHXNxcgw", "eZXwxvO1hvCY0KucrPfKo7yAyMT6Ajc3N7OkAB6VYy8"),
            });
        }

        [Fact]
        public void CanInstantiate()
        {
            // Arrange and Act 
            var es256Algorithm = new ES256Algorithm(_verifier.Object);
            // Assert
            Assert.NotNull(es256Algorithm);
        }

        [Fact]
        public void CanNotInstantiateIfVerifierNull()
        {
            // Arrange, Act, Assert 
            Assert.Throws<ArgumentNullException>(() => new ES256Algorithm(null));
        }

        [Fact]
        public void FromJwks_GivenValidJWKS_CanInstantiate()
        {
            // Arrange and Act
            var result = ES256Algorithm.FromJWKS(_kid, ValidJsonWebKeySet);

            // Assert
            Assert.True(result.Success && result.Value != null);
        }

        [Fact]
        public void FromJwks_GivenNoMatchingJWKS_CanNotInstantiate()
        {
            // Arrange and Act
            var result = ES256Algorithm.FromJWKS(_kid, JsonWebKeySetWithMissingKey);

            // Assert
            Assert.True(result.Failure);
        }

        [Fact]
        public void FromJwks_GivenInvalidJWKS_CanNotInstantiate()
        {
            // Arrange and Act
            var result = ES256Algorithm.FromJWKS(_kid, JsonWebKeySetWithBadXCoordinate);

            // Assert
            Assert.True(result.Failure);
        }

        [Fact]
        public void Verify_GivenValidPayloadAndSignature_CanValidate()
        {
            // Arrange
            var JwtPartsParseResult = JwsParts.ParseToken(ValidCompactJWS);
            var BytesToSign = Utf8EncodingSupport.GetBytes(JwtPartsParseResult.Value.Header, (byte)'.', JwtPartsParseResult.Value.Payload);
            var Signature = Base64UrlEncoder.Decode(JwtPartsParseResult.Value.Signature);
            var algorithm = ES256Algorithm.FromJWKS(_kid, ValidJsonWebKeySet).Value;

            // Act
            var result = algorithm.Verify(BytesToSign, Signature);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public void Verify_GivenModifiedPayload_CanNotValidate()
        {
            // Arrange
            var JwtPartsParseResult = JwsParts.ParseToken(CompactJWSWithModifiedPayload);
            var BytesToSign = Utf8EncodingSupport.GetBytes(JwtPartsParseResult.Value.Header, (byte)'.', JwtPartsParseResult.Value.Payload);
            var Signature = Base64UrlEncoder.Decode(JwtPartsParseResult.Value.Signature);
            var algorithm = ES256Algorithm.FromJWKS(_kid, ValidJsonWebKeySet).Value;

            // Act
            var result = algorithm.Verify(BytesToSign, Signature);

            // Assert
            Assert.True(result.Failure);
        }

        [Fact]
        public void Verify_GivenInvalidSignature_CanNotValidate()
        {
            // Arrange
            var JwtPartsParseResult = JwsParts.ParseToken(CompactJWSWithInvalidSignature);
            var BytesToSign = Utf8EncodingSupport.GetBytes(JwtPartsParseResult.Value.Header, (byte)'.', JwtPartsParseResult.Value.Payload);
            var Signature = Base64UrlEncoder.Decode(JwtPartsParseResult.Value.Signature);
            var algorithm = ES256Algorithm.FromJWKS(_kid, ValidJsonWebKeySet).Value;

            // Act
            var result = algorithm.Verify(BytesToSign, Signature);

            // Assert
            Assert.True(result.Failure);
        }
    }
}
