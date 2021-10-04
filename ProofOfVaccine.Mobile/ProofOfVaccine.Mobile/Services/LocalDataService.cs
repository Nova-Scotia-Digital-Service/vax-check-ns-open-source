using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PoV.Decode.DataStore;
using ProofOfVaccine.Token.Providers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ProofOfVaccine.Mobile.Services
{
    public interface ILocalDataService : IJwksDataStore
    {
        event EventHandler LastOnlineDateChanged;
        double GetDaysOffine();
        void SetLastOnlineDate();
    }

    public class LocalDataService : ILocalDataService
    {
        private readonly IErrorManagementService _errorManagementService;
        public LocalDataService()
        {
            _errorManagementService = DependencyService.Resolve<IErrorManagementService>();
        }

        #region Offline Store

        private const string lastOnlineKey = "last_online_date";

        public event EventHandler LastOnlineDateChanged;

        public double GetDaysOffine()
        {
            try
            {
                var lastOnlineDate = DateTimeOffset.Parse(Preferences.Get(lastOnlineKey, DateTimeOffset.UtcNow.ToString()));
                return (DateTimeOffset.UtcNow - lastOnlineDate).TotalDays;
            }
            catch (Exception ex)
            {
                _errorManagementService.HandleError(ex);
                return 0;
            }
        }

        public void SetLastOnlineDate()
        {
            Preferences.Set(lastOnlineKey, DateTimeOffset.UtcNow.ToString());
            LastOnlineDateChanged?.Invoke(this, new EventArgs());
        }
        #endregion

        #region JWKS Cache Store

        private const string jwksKey = "jwks_cache";

        public async Task<IJwksCache> LoadJWKS()
        {
            try
            {
                return JsonConvert.DeserializeObject<JwksCache>(await SecureStorage.GetAsync("jwks_cache"));

            }
            catch
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
            catch
            {
                // Possible that device doesn't support secure storage on device.
                return false;
            }
        }
        #endregion
    }
}
