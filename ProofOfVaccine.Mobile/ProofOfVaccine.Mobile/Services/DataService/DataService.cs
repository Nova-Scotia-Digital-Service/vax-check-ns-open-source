using Newtonsoft.Json;
using ProofOfVaccine.Mobile.DTOs;
using ProofOfVaccine.Token.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ProofOfVaccine.Mobile.Services
{
    public class DataService : IDataService
    {
        private Assembly _assembly;
        public DataService(Assembly assembly = null)
        {
            if (assembly == null)
                _assembly = IntrospectionExtensions.GetTypeInfo(typeof(DataService)).Assembly;
            else
                _assembly = assembly;
        }

        private Dictionary<Uri, JWKeySet> _jsonWebKeySets;
        public async Task<Dictionary<Uri, JWKeySet>> LoadJWKSFromOnlineAsync(Dictionary<Uri, JWKeySet> uriKeys)
        {//TODO
            //var request = new HttpRequestMessage(HttpMethod.Get, WellKnownUrl);
            //try
            //{
            //    using (HttpResponseMessage Response = await HttpClient.SendAsync(request, new CancellationToken()))
            //    {
            //        if (Response.StatusCode == HttpStatusCode.OK)
            //        {
            //            if (Response.Content == null)
            //            {
            //                return Result<JsonWebKeySet>.Fail("HttpClient response content was null");
            //            }
            //            using System.IO.Stream ResponseStream = await Response.Content.ReadAsStreamAsync();
            //            Result<JsonWebKeySet> JsonWebKeySetResult = JsonSerializer.FromJsonStream<JsonWebKeySet>(ResponseStream);
            //            if (JsonWebKeySetResult.Failure)
            //                return Result<JsonWebKeySet>.Fail($"Failed to deserialize the JsonWebKeySet (JWKS) which was returned from the endpoint {WellKnownUrl.OriginalString}. {JsonWebKeySetResult.Message}");

            //            return JsonWebKeySetResult;
            //        }
            //        else
            //        {
            //            string Message = string.Empty;
            //            if (Response.Content != null)
            //            {
            //                var ErrorResponseContent = await Response.Content.ReadAsStringAsync();
            //                return Result<JsonWebKeySet>.Fail($"Response status: {Response.StatusCode}, Content: {ErrorResponseContent}");
            //            }
            //            else
            //            {
            //                return Result<JsonWebKeySet>.Fail($"Response status: {Response.StatusCode}, Content: [None]");
            //            }
            //        }
            //    }
            //}
            //catch (HttpRequestException HttpRequestException)
            //{
            //    return Result<JsonWebKeySet>.Retry($"HttpRequestException when calling the API: {HttpRequestException.Message}");
            //}
            //catch (TimeoutException)
            //{
            //    return Result<JsonWebKeySet>.Retry("TimeoutException during call to API");
            //}
            //catch (OperationCanceledException)
            //{
            //    return Result<JsonWebKeySet>.Retry("Task was canceled during call to API");
            //}
            //catch (Exception e)
            //{
            //    return Result<JsonWebKeySet>.Retry($"Task failed with due to unexpected exception of type {e.GetType()}.");
            //}
            return null;
        }
        public async Task<Dictionary<Uri, JWKeySet>> GetJWKSLocallyAsync()
        {
            if (_jsonWebKeySets == null)
                _jsonWebKeySets = await LoadJWKSFromDiskAsync();

            return _jsonWebKeySets;
        }
        public async Task<Dictionary<Uri, JWKeySet>> LoadJWKSFromDiskAsync()
        {//TODO
            try
            {
                var data = await SecureStorage.GetAsync(nameof(_jsonWebKeySets));
                return JsonConvert.DeserializeObject<Dictionary<Uri, JWKeySet>>(data);
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<Dictionary<Uri, JWKeySet>> SaveJWKSAsync(Dictionary<Uri, JWKeySet> keySets)
        {//TODO
            try
            {
                await SecureStorage.SetAsync(nameof(_jsonWebKeySets), JsonConvert.SerializeObject(keySets));
                return keySets;
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        private List<Vaccine> _validVaccines;
        public List<Vaccine> GetValidVaccines(bool forceLoadfromfile = false)
        {
            if (_validVaccines == null)
                _validVaccines = new List<Vaccine>();

            if (_validVaccines.Count <= 0 || forceLoadfromfile)
            {
                Stream stream = _assembly.GetManifestResourceStream("ProofOfVaccine.Mobile.AppResources.ValidVaccines.json");

                using (var reader = new StreamReader(stream))
                {
                    _validVaccines = JsonConvert.DeserializeObject<List<Vaccine>>(reader.ReadToEnd());
                }
            }

            return _validVaccines;
        }

        private Dictionary<Uri, JWKeySet> _whitelistedIssuerKeySets;
        public Dictionary<Uri, JWKeySet> GetWhitelistedIssuerKeySets(bool forceLoadfromfile = false)
        {
            if (_whitelistedIssuerKeySets == null)
                _whitelistedIssuerKeySets = new Dictionary<Uri, JWKeySet>();

            if (_whitelistedIssuerKeySets.Count <= 0 || forceLoadfromfile)
            {
                Stream stream = _assembly.GetManifestResourceStream("ProofOfVaccine.Mobile.AppResources.WhiteList.json");

                using (var reader = new StreamReader(stream))
                {
                    _whitelistedIssuerKeySets = JsonConvert.DeserializeObject<Dictionary<Uri, JWKeySet>>(reader.ReadToEnd());
                }

            }

            return _whitelistedIssuerKeySets;
        }

    }
}
