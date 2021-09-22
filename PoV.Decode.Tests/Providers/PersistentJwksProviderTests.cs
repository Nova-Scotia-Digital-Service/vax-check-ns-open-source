using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using PoV.Decode.DataStore;
using PoV.Decode.Exceptions;
using PoV.Decode.Providers;
using ProofOfVaccine.Token.Model.Jwks;
using ProofOfVaccine.Token.Providers;
using ProofOfVaccine.Token.Serializers.Json;
using ProofOfVaccine.Token.Support;
using Xunit;

namespace PoV.Decode.Tests.Providers
{
    public class PersistentJwksProviderTests : IAsyncLifetime
    {
        private readonly Mock<IJwksDataStore> _jwksDataStore;
        private readonly Mock<IHttpClient> _httpClient;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly TimeSpan _defaultTimeSpan = TimeSpan.FromDays(30);
        private readonly JwksCache _defaultCache;

        private readonly Uri URI_1 = new Uri("https://smarthealth.cards/ns/issuer");
        private const string JWKS_1 = @"
{
'keys':[
    {
      'kty': 'EC',
      'kid': '_IY9W2kRRFUigDfSB9r8jHgMRrT0w4p5KN93nGThdH8',
      'use': 'sig',
      'alg': 'ES256',
      'crv': 'P-256',
      'x': '7xbC_9ZmFwKqOHpwX6-LnlhIh5SMIuNwl0PW1yVI_sk',
      'y': '7k2fdIRNDHdf93vL76wxdXEPtj_GiMTTyecm7EUUMQo',
    }
  ]
}
";

        // Two keys added, one removed.
        private const string JWKS_1_UPDATED = @"
{
'keys':[
    {
      'kty': 'EC',
      'kid': '_IY9W2kRRFUigDfSB9r8notthesamep5KN93nGThdH8',
      'use': 'sig',
      'alg': 'ES256',
      'crv': 'P-256',
      'x': '7xbC_9ZmFwKqOHpwX6-LnlhIh5SMIuNwl0PW1yVI_sk',
      'y': '7k2fdIRNDHdf93vL76wxdXEPtj_GiMTTyecm7EUUMQo',
    },
    {
      'kty': 'EC',
      'kid': '_IY9W2kRRFUigDfSalsonotthesamep5KN93nGThdH8',
      'use': 'sig',
      'alg': 'ES256',
      'crv': 'P-256',
      'x': '7xbC_9ZmFwKqOHpwX6-LnlhIh5SMIuNwl0PW1yVI_sk',
      'y': '7k2fdIRNDHdf93vL76wxdXEPtj_GiMTTyecm7EUUMQo',
    },
  ]
}
";

        private readonly Uri URI_2 = new Uri("https://smarthealth.cards/nb/issuer");
        private const string JWKS_2 = @"
{
'keys':[
    {
      'kty': 'EC',
      'kid': '_IY9W2kRRFUigDfSB9r8jHgMRrT0w4p5KN93nGThdH8',
      'use': 'sig',
      'alg': 'ES256',
      'crv': 'P-256',
      'x': '7xbC_9ZmFwKqOHpwX6-LnlhIh5SMIuNwl0PW1yVI_sk',
      'y': '7k2fdIRNDHdf93vL76wxdXEPtj_GiMTTyecm7EUUMQo',
    }
  ]
}
";

        private readonly Uri URI_3 = new Uri("https://smarthealth.cards/malicious/issuer");
        private readonly Uri BAD_URI = new Uri("https://smarthealth.cards/missing/issuer");

        public PersistentJwksProviderTests()
        {
            _httpClient = new Mock<IHttpClient>();
            _jsonSerializer = new JsonSerializer();
            _jwksDataStore = new Mock<IJwksDataStore>();
            _defaultCache = new JwksCache(TimeSpan.FromDays(365));

            _defaultCache.Set(URI_1, new JsonWebKeySet(new List<JsonWebKey>
                {
                    new JsonWebKey("EC", "3Kfdg-XwP-7gXyywtUfUADwBumDOPKMQx-iELL11W9s", "sig", "ES256", "P-256", "11XvRWy1I2S0EyJlyf_bWfw_TQ5CJJNLw78bHXNxcgw", "eZXwxvO1hvCY0KucrPfKo7yAyMT6Ajc3N7OkAB6VYy8")
                }));

            _jwksDataStore
                .Setup(jwkds => jwkds.LoadJWKS())
                .ReturnsAsync(() =>
                {
                    var jwksCache = new JwksCache(TimeSpan.FromDays(30));
                    jwksCache.Set(URI_1, new JsonWebKeySet(new List<JsonWebKey>
                    {
                        new JsonWebKey("EC", "_IY9W2kRRFUigDfSB9r8jHgMRrT0w4p5KN93nGThdH8", "sig", "ES256", "P-256", "7xbC_9ZmFwKqOHpwX6-LnlhIh5SMIuNwl0PW1yVI_sk", "7k2fdIRNDHdf93vL76wxdXEPtj_GiMTTyecm7EUUMQo")
                    }));
                    jwksCache.Set(URI_2, new JsonWebKeySet(new List<JsonWebKey>
                    {
                        new JsonWebKey("EC", "_IY9W2kRRFUigDfSB9r8jHgMRrT0w4p5KN93nGThdH8", "sig", "ES256", "P-256", "7xbC_9ZmFwKqOHpwX6-LnlhIh5SMIuNwl0PW1yVI_sk", "7k2fdIRNDHdf93vL76wxdXEPtj_GiMTTyecm7EUUMQo")
                    }));
                    return jwksCache;
                });

            _jwksDataStore
                .Setup(jwkds => jwkds.StoreJWKS(It.IsAny<JwksCache>()))
                .ReturnsAsync(() => true);

            _httpClient
                .Setup(hc => hc.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((HttpRequestMessage rm, CancellationToken c) => {
                    if (rm.RequestUri.OriginalString == URI_1.OriginalString)
                    {
                        return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                        {
                            Content = new StringContent(JWKS_1)
                        };
                    }
                    else if(rm.RequestUri.OriginalString == URI_2.OriginalString)
                    {
                        return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                        {
                            Content = new StringContent(JWKS_2)
                        };
                    }

                    return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
 
                });
        }

        [Fact]
        public void CanInstantiate()
        {
            // Arrange and Act
            PersistentJwksProvider persistentJwksProvider = new PersistentJwksProvider(_jwksDataStore.Object, null);
            // Assert
            Assert.NotNull(persistentJwksProvider);
        }

        [Fact]
        public void CanNotInstantiateWithoutDataStore()
        {
            // Arrange, Act, and Assert
            Assert.Throws<ArgumentException>(() => new PersistentJwksProvider(null, null));
        }

        [Fact]
        public async Task TryInitializeJwksAsync_GivenNoConnectivity_JwksCacheIsLoaded()
        {
            // Arrange
            List<Uri> whiteList = new List<Uri>()
            {
                URI_1,
                URI_2
            };
            PersistentJwksProvider persistentJwksProvider = new PersistentJwksProvider(_httpClient.Object, _jsonSerializer, _defaultTimeSpan, _jwksDataStore.Object, whiteList, _defaultCache);

            // Act
            IList<Result<JsonWebKeySet>> results = await persistentJwksProvider.TryInitializeJwksAsync(false);

            // Assert
            Assert.Collection(results,
                r => Assert.True(r.Success),
                r => Assert.True(r.Success));
        }

        [Fact]
        public async Task TryInitializeJwksAsync_GivenNoConnectivityAndNoPersistentDataIsUsed_DefaultCacheIsUsed()
        {
            // Arrange
            _jwksDataStore
                .Setup(jwkds => jwkds.LoadJWKS())
                .ReturnsAsync(() =>
                {
                    return null;
                });
            List<Uri> whiteList = new List<Uri>()
            {
                URI_1,
                URI_2
            };
            PersistentJwksProvider persistentJwksProvider = new PersistentJwksProvider(_httpClient.Object, _jsonSerializer, _defaultTimeSpan, _jwksDataStore.Object, whiteList, _defaultCache);

            // Act
            IList<Result<JsonWebKeySet>> results = await persistentJwksProvider.TryInitializeJwksAsync(false);

            // Assert
            Assert.Collection(results,
                r => Assert.True(r.Success),
                r => Assert.True(r.Failure));
        }

        [Fact]
        public async Task TryInitializeJwksAsync_GivenDataStoreIsMissingDataForWhiteListAndNoConnectivity_MissingJwksIsNotFound()
        {
            // Arrange
            // Override default to have URI_2 missing from cache.
            _jwksDataStore
                .Setup(jwkds => jwkds.LoadJWKS())
                .ReturnsAsync(() =>
                {
                    var jwksCache = new JwksCache(TimeSpan.FromDays(30));
                    jwksCache.Set(URI_1, new JsonWebKeySet(new List<JsonWebKey>
                    {
                        new JsonWebKey("EC", "_IY9W2kRRFUigDfSB9r8jHgMRrT0w4p5KN93nGThdH8", "sig", "ES256", "P-256", "7xbC_9ZmFwKqOHpwX6-LnlhIh5SMIuNwl0PW1yVI_sk", "7k2fdIRNDHdf93vL76wxdXEPtj_GiMTTyecm7EUUMQo")
                    }));
                    return jwksCache;
                });

            List<Uri> whiteList = new List<Uri>()
            {
                URI_1,
                URI_2
            };

            PersistentJwksProvider persistentJwksProvider = new PersistentJwksProvider(_httpClient.Object, _jsonSerializer, _defaultTimeSpan, _jwksDataStore.Object, whiteList, _defaultCache);

            // Act
            IList<Result<JsonWebKeySet>> results = await persistentJwksProvider.TryInitializeJwksAsync(false);

            // Assert
            Assert.Collection(results,
                r => Assert.True(r.Success),
                r => Assert.True(r.Failure));
        }

        [Fact]
        public async Task TryInitializeJwksAsync_GivenDataStoreIsMissingDataForWhiteListAndHasConnectivity_NewJwksIsLoadedFromHTTPS()
        {
            // Arrange
            // Override default to have URI_2 missing from cache.
            _jwksDataStore
                .Setup(jwkds => jwkds.LoadJWKS())
                .ReturnsAsync(() =>
                {
                    var jwksCache = new JwksCache(TimeSpan.FromDays(30));
                    jwksCache.Set(URI_1, new JsonWebKeySet(new List<JsonWebKey>
                    {
                        new JsonWebKey("EC", "_IY9W2kRRFUigDfSB9r8jHgMRrT0w4p5KN93nGThdH8", "sig", "ES256", "P-256", "7xbC_9ZmFwKqOHpwX6-LnlhIh5SMIuNwl0PW1yVI_sk", "7k2fdIRNDHdf93vL76wxdXEPtj_GiMTTyecm7EUUMQo")
                    }));
                    return jwksCache;
                });

            List<Uri> whiteList = new List<Uri>()
            {
                URI_1,
                URI_2
            };

            PersistentJwksProvider persistentJwksProvider = new PersistentJwksProvider(_httpClient.Object, _jsonSerializer, _defaultTimeSpan, _jwksDataStore.Object, whiteList, _defaultCache);

            // Act
            IList<Result<JsonWebKeySet>> results = await persistentJwksProvider.TryInitializeJwksAsync(true);

            // Assert
            Assert.Collection(results,
                r => Assert.True(r.Success),
                r => Assert.True(r.Success));
        }

        [Fact]
        public async Task TryInitializeJwksAsync_GivenConnectivityAndFailedRetryableHTTPSCalls_JwksIsEventuallyLoadedFromHTTPS()
        {
            // Arrange
            // Override the http client to fail in a retryable way twice.
            var retryCount = 0;
            _httpClient
                .Setup(hc => hc.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((HttpRequestMessage rm, CancellationToken c) => {
                    if(retryCount < 3)
                    {
                        retryCount++;
                        throw new HttpRequestException("test");
                    }

                    return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                    {
                        Content = new StringContent(JWKS_1)
                    };
                });

            List<Uri> whiteList = new List<Uri>()
            {
                URI_1,
            };

            PersistentJwksProvider persistentJwksProvider = new PersistentJwksProvider(_httpClient.Object, _jsonSerializer, _defaultTimeSpan, _jwksDataStore.Object, whiteList, _defaultCache);

            // Act
            IList<Result<JsonWebKeySet>> results = await persistentJwksProvider.TryInitializeJwksAsync(true);

            // Assert
            Assert.Collection(results,
                r => Assert.True(r.Success));
        }

        [Fact]
        public async Task TryInitializeJwksAsync_GivenDataStoreIsMissingDataForWhiteListAndUriIsBad_FailsToLoadJWKSForURI()
        {
            // Arrange
            // Override default to have URI_2 missing from cache.
            _jwksDataStore
                .Setup(jwkds => jwkds.LoadJWKS())
                .ReturnsAsync(() =>
                {
                    var jwksCache = new JwksCache(TimeSpan.FromDays(30));
                    jwksCache.Set(URI_1, new JsonWebKeySet(new List<JsonWebKey>
                    {
                            new JsonWebKey("EC", "_IY9W2kRRFUigDfSB9r8jHgMRrT0w4p5KN93nGThdH8", "sig", "ES256", "P-256", "7xbC_9ZmFwKqOHpwX6-LnlhIh5SMIuNwl0PW1yVI_sk", "7k2fdIRNDHdf93vL76wxdXEPtj_GiMTTyecm7EUUMQo")
                    }));
                    return jwksCache;
                });

            List<Uri> whiteList = new List<Uri>()
            {
                URI_1,
                BAD_URI
            };

            PersistentJwksProvider persistentJwksProvider = new PersistentJwksProvider(_httpClient.Object, _jsonSerializer, _defaultTimeSpan, _jwksDataStore.Object, whiteList, _defaultCache);

            // Act
            IList<Result<JsonWebKeySet>> results = await persistentJwksProvider.TryInitializeJwksAsync(true);

            // Assert
            Assert.Collection(results,
                r => Assert.True(r.Success),
                r => Assert.True(r.Failure));
        }

        [Fact]
        public async Task TryInitializeJwksAsync_GivenDataStoreIsContainsDataThatHasBeenUpdatedOnServerAndHasConnectivity_ResultsContainsDetailsOnChange()
        {
            // Arrange
            // Override default to have URI_2 missing from cache.
            _jwksDataStore
                .Setup(jwkds => jwkds.LoadJWKS())
                .ReturnsAsync(() =>
                {
                    var jwksCache = new JwksCache(TimeSpan.FromDays(30));
                    jwksCache.Set(URI_1, new JsonWebKeySet(new List<JsonWebKey>
                    {
                            new JsonWebKey("EC", "_IY9W2kRRFUigDfSB9r8jHgMRrT0w4p5KN93nGThdH8", "sig", "ES256", "P-256", "7xbC_9ZmFwKqOHpwX6-LnlhIh5SMIuNwl0PW1yVI_sk", "7k2fdIRNDHdf93vL76wxdXEPtj_GiMTTyecm7EUUMQo")
                    }));
                    return jwksCache;
                });

            _httpClient
                .Setup(hc => hc.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((HttpRequestMessage rm, CancellationToken c) => {

                    return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                    {
                        Content = new StringContent(JWKS_1_UPDATED)
                    };
                });

            List<Uri> whiteList = new List<Uri>()
            {
                URI_1,
            };

            PersistentJwksProvider persistentJwksProvider = new PersistentJwksProvider(_httpClient.Object, _jsonSerializer, _defaultTimeSpan, _jwksDataStore.Object, whiteList, _defaultCache);

            // Act
            IList<Result<JsonWebKeySet>> results = await persistentJwksProvider.TryInitializeJwksAsync(true);

            // Assert
            Assert.Collection(results,
                r => Assert.True(r.Success && r.Message.Length > 100));
        }

        [Fact]
        public async Task GetJwksAsync_GivenUriNotInCache_ResultIsFailure()
        {
            // Arrange
            List<Uri> whiteList = new List<Uri>()
            {
                URI_1,
                URI_2
            };
            PersistentJwksProvider persistentJwksProvider = new PersistentJwksProvider(_httpClient.Object, _jsonSerializer, _defaultTimeSpan, _jwksDataStore.Object, whiteList, _defaultCache);

            // Act
            await persistentJwksProvider.TryInitializeJwksAsync(true);
            Result<JsonWebKeySet> result = await persistentJwksProvider.GetJwksAsync(URI_3);

            // Assert
            Assert.True(result.Failure);
        }

        [Fact]
        public async Task GetJwksAsync_GivenUriExistsInCache_ResultIsSuccess()
        {
            // Arrange
            List<Uri> whiteList = new List<Uri>()
            {
                URI_1,
                URI_2
            };

            PersistentJwksProvider persistentJwksProvider = new PersistentJwksProvider(_httpClient.Object, _jsonSerializer, _defaultTimeSpan, _jwksDataStore.Object, whiteList, _defaultCache);

            // Act
            await persistentJwksProvider.TryInitializeJwksAsync(true);
            Result<JsonWebKeySet> result = await persistentJwksProvider.GetJwksAsync(URI_1);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task GetJwksAsync_GivenTryInitializeJwksAsyncNotCalled_ThrowsException()
        {
            // Arrange
            List<Uri> whiteList = new List<Uri>()
            {
                URI_1,
                URI_2
            };

            PersistentJwksProvider persistentJwksProvider = new PersistentJwksProvider(_httpClient.Object, _jsonSerializer, _defaultTimeSpan, _jwksDataStore.Object, whiteList, _defaultCache);

            // Act and assert
            await Assert.ThrowsAsync<JwksCacheNotInitializedException<IJwksDataStore>>(() => persistentJwksProvider.GetJwksAsync(URI_1));
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
