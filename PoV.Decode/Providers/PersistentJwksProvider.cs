using PoV.Decode.DataStore;
using PoV.Decode.Exceptions;
using ProofOfVaccine.Token.Model.Jwks;
using ProofOfVaccine.Token.Providers;
using ProofOfVaccine.Token.Serializers.Json;
using ProofOfVaccine.Token.Support;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using System.Linq;

namespace PoV.Decode.Providers
{
    /// <summary>
    /// A special type of <see cref="IJwksProvider"/> that allows for <see cref="JwksCache"/> persistance.
    /// This Provider loads the cache from a persistent <see cref="IJwksDataStore"/>.
    /// </summary>
    public class PersistentJwksProvider : IPersistentJwksProvider<IJwksDataStore>
    {
        /// <summary>
        /// The amount of times the provider will retry attempting to access the JWKS via HTTPS.
        /// </summary>
        private const int RETRY_COUNT = 3;

        /// <summary>
        /// A flag used to determine whether or not the cache has been loaded in.
        /// </summary>
        private bool _providerCacheLoadAttempted = false;

        /// <summary>
        /// The data store used to hold onto the <see cref="JwksCache"/> persistently.
        /// </summary>
        private readonly IJwksDataStore DataStore;

        /// <summary>
        /// The <see cref="IHttpClient"/> used to find the white-listed JWKS in the case that it doesn't exist in the <see cref="DataStore"/>.
        /// </summary>
        private readonly IHttpClient HttpClient;

        /// <summary>
        /// The JSON Serializer used to serialize responses from the HTTPS requests to well known URLs.
        /// </summary>
        private readonly IJsonSerializer JsonSerializer;

        /// <summary>
        /// The maximum amount of time that a item in the cache can exist.
        /// </summary>
        private readonly TimeSpan? MaxCacheLife;

        /// <summary>
        /// A set of <see cref="Uri"/> that the provider is allowed to attempt to find JWKS for.
        /// This set is used from <see cref="Try"/>
        /// </summary>
        private readonly IList<Uri> WhiteListedUriSet;

        /// <summary>
        /// The cached JWKS. Loaded in from the DataStore, or HTTPS failing that.
        /// </summary>
        private IJwksCache JwksCache;

        /// <summary>
        /// Returns the amount of time that should be waited before making another HTTPS request based on the number of attempts.
        /// </summary>
        /// <param name="attempts"></param>
        /// <returns></returns>
        private static TimeSpan PollyRetryAttempt(int attempts) => TimeSpan.FromSeconds(Math.Pow(2, attempts));

        /// <summary>
        /// Constructor for setting up with the default <see cref="MaxCacheLife"/>.
        /// </summary>
        /// <param name="DataStore"></param>
        public PersistentJwksProvider(IJwksDataStore DataStore, IList<Uri> WhiteListedUriSet, JwksCache DefaultCache = null)
          : this(null, null, null, DataStore, WhiteListedUriSet, DefaultCache) { }

        /// <summary>
        /// The full constructor.
        /// </summary>
        /// <param name="HttpClient">The client used for HTTPS calls.</param>
        /// <param name="JsonSerializer">The JSON serializer used to deserialize the HTTP response.</param>
        /// <param name="MaxCacheLife">The maximum amount of time a JWKS can sit in the cache.</param>
        /// <param name="DataStore">The data store used to persist the cache between sessions.</param>
        /// <param name="Defaults">The default <see cref="JsonWebKeySet"/> that should be loaded into the cache regardless of existing cache or JWKS found via HTTPS.</param>
        public PersistentJwksProvider(IHttpClient? HttpClient, IJsonSerializer? JsonSerializer, TimeSpan? MaxCacheLife, IJwksDataStore? DataStore, IList<Uri>? WhiteListedUriSet, JwksCache? DefaultCache)
        {
            this.HttpClient = HttpClient ?? ProofOfVaccine.Token.Providers.HttpClient.Create();
            this.JsonSerializer = JsonSerializer ?? new JsonSerializer();
            this.MaxCacheLife = MaxCacheLife ?? TimeSpan.FromDays(365); // Should persist nearly indefinitely.
            this.DataStore = DataStore ?? throw new ArgumentException($"A {nameof(IJwksDataStore)} is a necessity for a {nameof(PersistentJwksProvider)}.");
            this.JwksCache = DefaultCache ?? new JwksCache(MaxCacheLife);
            this.WhiteListedUriSet = WhiteListedUriSet ?? new List<Uri>();
        }

        /// <summary>
        /// Always attempt to load from the cache.
        /// </summary>
        /// <exception cref="JwksCacheNotInitializedException">
        /// This cache must be preloaded by calling <see cref="TryInitializeJwksAsync(IList{Uri}, bool)"/> before this method can be used.
        /// </exception>
        public Task<Result<JsonWebKeySet>> GetJwksAsync(Uri WellKnownUrl, CancellationToken? CancellationToken = null)
        {
            if (!_providerCacheLoadAttempted)
            {
                throw new JwksCacheNotInitializedException<IJwksDataStore>();
            }

            //Check the Cache for the target
            Result<JsonWebKeySet> CacheResult = JwksCache.Get(WellKnownUrl);
            if (CacheResult.Success)
                return Task.FromResult(CacheResult);

            return Task.FromResult(Result<JsonWebKeySet>.Fail("Uri was not found in the cache."));
        }

        public async Task<IList<Result<JsonWebKeySet>>> TryInitializeJwksAsync(bool HasConnectivity)
        {
            _providerCacheLoadAttempted = true;

            var loadedJwksResults = new List<Result<JsonWebKeySet>>();

            // If the JwksCache is already in the data store retrieve it.
            if ((await DataStore.LoadJWKS()) is JwksCache retrievedJwkCache)
            {
                JwksCache = retrievedJwkCache;
                foreach(var result in JwksCache.ClearStale(WhiteListedUriSet))
                {
                    result.ExpandOnMessage($"JWKS was removed from the cache due to no longer being whitelisted.");
                    loadedJwksResults.Add(result);
                }
            }

            if (!HasConnectivity)
            {
                // Verify each whitelisted URL in exists in cache if we have no connectivity.
                foreach (var wellKnownUrl in WhiteListedUriSet)
                {
                    loadedJwksResults.Add(JwksCache.Get(wellKnownUrl));
                }
            }
            else
            {
                // Verify that the white-listed URLs are updated via HTTPS if we have connectivity. 
                foreach (var wellKnownUrl in WhiteListedUriSet)
                {
                    // Attempt to update or find new JWKS if possible.
                    var httpsResult = await Policy.HandleResult<Result<JsonWebKeySet>>(r => r.Retryable)
                        .WaitAndRetryAsync(RETRY_COUNT, PollyRetryAttempt)
                        .ExecuteAsync(() => TryInitializeJWKSFromWellKnownUrl(wellKnownUrl));

                    // If we are replacing an item that is cached, log some details on changes if they exist.
                    if (httpsResult.Success)
                    { 
                        if(JwksCache.Get(wellKnownUrl) is Result<JsonWebKeySet> oldCachedJwks && oldCachedJwks.Success)
                        {
                            //TODO: Look more into expanding the logging here to verify whether the IP Address of the public URL has change.
                            httpsResult.ExpandOnMessage($"JWKS from {wellKnownUrl} updated on {DateTimeOffset.Now}. ");
                            foreach (var key in httpsResult.Value.Keys.Except(oldCachedJwks.Value.Keys))
                            {
                                httpsResult.ExpandOnMessage($"The JWKS at {wellKnownUrl} contains a new key with kid of {key.Kid}. ");
                            }

                            foreach (var key in oldCachedJwks.Value.Keys.Except(httpsResult.Value.Keys))
                            {
                                httpsResult.ExpandOnMessage($"A key with kid of {key.Kid} has been removed from the JWKS at {wellKnownUrl}. ");
                            }
                        }

                        //Store the JsonWebKeySet in the cache
                        JwksCache.Set(wellKnownUrl, httpsResult.Value);
                    }

                    loadedJwksResults.Add(httpsResult);
                }

            }


            // Store the updated cache in the data store for next time they are loaded.
            await TrySaveJwksToPersistentStore();

            return loadedJwksResults;
        }

        /// <summary>
        /// Attempt to retrieve the JWKS from the <paramref name="WellKnownUrl"/> via HTTPS.
        /// </summary>
        /// <param name="WellKnownUrl">A white-listed URI to a JWKS.</param>
        /// <param name="CancellationToken">A cancellation token.</param>
        /// <returns>The results of the HTTPS request.</returns>
        private async Task<Result<JsonWebKeySet>> TryInitializeJWKSFromWellKnownUrl(Uri WellKnownUrl, CancellationToken? CancellationToken = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, WellKnownUrl);
            try
            {
                using (HttpResponseMessage Response = await HttpClient.SendAsync(request, CancellationToken ?? new CancellationToken()))
                {
                    if (Response.StatusCode == HttpStatusCode.OK)
                    {
                        if (Response.Content == null)
                        {
                            return Result<JsonWebKeySet>.Fail("HttpClient response content was null");
                        }
                        using System.IO.Stream ResponseStream = await Response.Content.ReadAsStreamAsync();
                        Result<JsonWebKeySet> JsonWebKeySetResult = JsonSerializer.FromJsonStream<JsonWebKeySet>(ResponseStream);
                        if (JsonWebKeySetResult.Failure)
                            return Result<JsonWebKeySet>.Fail($"Failed to deserialize the JsonWebKeySet (JWKS) which was returned from the endpoint {WellKnownUrl.OriginalString}. {JsonWebKeySetResult.Message}");

                        return JsonWebKeySetResult;
                    }
                    else
                    {
                        string Message = string.Empty;
                        if (Response.Content != null)
                        {
                            var ErrorResponseContent = await Response.Content.ReadAsStringAsync();
                            return Result<JsonWebKeySet>.Fail($"Response status: {Response.StatusCode}, Content: {ErrorResponseContent}");
                        }
                        else
                        {
                            return Result<JsonWebKeySet>.Fail($"Response status: {Response.StatusCode}, Content: [None]");
                        }
                    }
                }
            }
            catch (HttpRequestException HttpRequestException)
            {
                return Result<JsonWebKeySet>.Retry($"HttpRequestException when calling the API: {HttpRequestException.Message}");
            }
            catch (TimeoutException)
            {
                return Result<JsonWebKeySet>.Retry("TimeoutException during call to API");
            }
            catch (OperationCanceledException)
            {
                return Result<JsonWebKeySet>.Retry("Task was canceled during call to API");
            }
            catch (Exception e)
            {
                return Result<JsonWebKeySet>.Retry($"Task failed with due to unexpected exception of type {e.GetType()}.");
            }
        }

        /// <summary>
        /// Attempt to save the currt <see cref="JwksCache"/>.
        /// </summary>
        /// <returns>True if successful, false if a failure.</returns>
        private Task<bool> TrySaveJwksToPersistentStore()
        {
            return DataStore.StoreJWKS(JwksCache);
        }
    }
}
