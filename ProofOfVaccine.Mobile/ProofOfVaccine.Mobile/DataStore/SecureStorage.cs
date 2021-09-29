using System;
using System.Threading.Tasks;
using PoV.Decode.DataStore;
using ProofOfVaccine.Token.Providers;
using Xamarin.Essentials;
using Newtonsoft.Json;

namespace ProofOfVaccine.Mobile.DataStore
{
    public class SecureStore : IJwksDataStore
    {
        public async Task<IJwksCache> LoadJWKS()
        {
            try
            {
                return JsonConvert.DeserializeObject<JwksCache>(await SecureStorage.GetAsync("jwks_cache"));
               
            }
            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
                return null;
            }
        }

        public async Task<bool> StoreJWKS(IJwksCache cache)
        {
            try
            {
                await SecureStorage.SetAsync("jwks_cache", JsonConvert.SerializeObject(cache));
                return true;
            }
            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
                return false;
            }
        }
    }
}
