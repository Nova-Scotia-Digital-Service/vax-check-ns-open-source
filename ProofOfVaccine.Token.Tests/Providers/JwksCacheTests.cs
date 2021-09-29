using System;
using System.Collections.Generic;
using ProofOfVaccine.Token.Model.Jwks;
using ProofOfVaccine.Token.Providers;
using Xunit;

namespace ProofOfVaccine.Token.Tests.Providers
{
    public class JwksCacheTests
    {
        private readonly IList<Uri> _whiteList;

        public JwksCacheTests()
        {
            _whiteList= new List<Uri>
            {
                new Uri("https://pvc-dev.novascotia.ca/issuer/.well-known/jwks.json"),
                new Uri("https://pvc.novascotia.ca/issuer/.well-known/jwks.json"),
                new Uri("https://old.novascotia.ca/issuer/.well-known/jwks.json")
            };
        }

        [Fact]
        public void ClearStale_GivenANewWhiteListWithRemovedJwks_OldJwksIsReturned()
        {
            // Arrange
            var jwks1 = new JsonWebKeySet();
            var jwks2 = new JsonWebKeySet();
            var jwks3 = new JsonWebKeySet();
            JwksCache jwksCache = new JwksCache(TimeSpan.MaxValue);
            jwksCache.Set(_whiteList[0], jwks1);
            jwksCache.Set(_whiteList[1], jwks2);
            jwksCache.Set(_whiteList[2], jwks3);
            _whiteList.RemoveAt(2);

            // Act
            var results = jwksCache.ClearStale(_whiteList);

            //Assert
            Assert.Collection(results,
                r => Assert.True(r.Success && r.Value == jwks3));
        }

        [Fact]
        public void ClearStale_GivenANewWhiteListWithRemovedJwks_OldJwksIsRemovedFromCache()
        {
            // Arrange
            var oldUri = _whiteList[2];
            var jwks1 = new JsonWebKeySet();
            var jwks2 = new JsonWebKeySet();
            var jwks3 = new JsonWebKeySet();

            JwksCache jwksCache = new JwksCache(TimeSpan.MaxValue);
            jwksCache.Set(_whiteList[0], jwks1);
            jwksCache.Set(_whiteList[1], jwks2);
            jwksCache.Set(_whiteList[2], jwks3);
            _whiteList.RemoveAt(2);

            // Act
            jwksCache.ClearStale(_whiteList);

            //Assert
            Assert.True(jwksCache.Get(oldUri).Failure);
        }

        [Fact]
        public void ClearStale_GivenANewWhiteListWithRemovedJwks_WhitelistedJwksRemainInCache()
        {
            // Arrange
            var oldUri = _whiteList[2];
            var jwks1 = new JsonWebKeySet();
            var jwks2 = new JsonWebKeySet();
            var jwks3 = new JsonWebKeySet();

            JwksCache jwksCache = new JwksCache(TimeSpan.MaxValue);
            jwksCache.Set(_whiteList[0], jwks1);
            jwksCache.Set(_whiteList[1], jwks2);
            jwksCache.Set(_whiteList[2], jwks3);
            _whiteList.RemoveAt(2);

            // Act
            jwksCache.ClearStale(_whiteList);

            //Assert
            Assert.True(jwksCache.Get(_whiteList[0]).Success);
            Assert.True(jwksCache.Get(_whiteList[1]).Success);
        }
    }
}
