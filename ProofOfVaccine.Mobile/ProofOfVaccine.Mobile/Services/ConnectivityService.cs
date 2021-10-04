using ProofOfVaccine.Mobile.DTOs;
using ProofOfVaccine.Mobile.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ProofOfVaccine.Mobile.Services
{
    public interface IConnectivityService
    {
        event ConnectivityService.NetworkConnectivityEventHandler NetworkConnectivityChanged;
        NetworkInfo GetNetworkInfo { get; }
        bool ListenForConnectivityChange { get; set; }


        void EnableConnectivityUpdates();
        void DisableConnectivityUpdates();
    }


    public class ConnectivityService : IConnectivityService
    {
        private bool _canListeningForConnectivityChanges = false;
        public bool ListenForConnectivityChange
        {
            get => _canListeningForConnectivityChanges;
            set
            {
                if (_canListeningForConnectivityChanges == value) return;

                if (value)
                    EnableConnectivityUpdates();
                else
                    DisableConnectivityUpdates();

                _canListeningForConnectivityChanges = value;
            }

        }

        public delegate void NetworkConnectivityEventHandler(object sender, NetworkInfo args);
        public event NetworkConnectivityEventHandler NetworkConnectivityChanged;

        public bool IsNetworkAvailable => Connectivity.NetworkAccess == NetworkAccess.Internet ||
                                          Connectivity.NetworkAccess == NetworkAccess.ConstrainedInternet;

        private NetworkInfo _networkInfo;
        public NetworkInfo GetNetworkInfo => _networkInfo;

        protected readonly IErrorManagementService _errorManagementService;
        protected readonly IDataService _dataService;
        public ConnectivityService(bool enableListener = true)
        {
            _dataService = DependencyService.Resolve<IDataService>();
            _errorManagementService = DependencyService.Resolve<IErrorManagementService>();

            NetworkConnectivityChanged += OnNetworkConnectivityChanged;
            ListenForConnectivityChange = enableListener;
        }
        ~ConnectivityService()
        {
            NetworkConnectivityChanged -= OnNetworkConnectivityChanged;
        }

        private void OnNetworkConnectivityChanged(object sender, NetworkInfo info)
        {
            _networkInfo = info;
        }

        public void EnableConnectivityUpdates()
        {
            Connectivity.ConnectivityChanged += OnConnectivityChanged;

            UpdateLastOnline(Connectivity.NetworkAccess);
        }

        public void DisableConnectivityUpdates()
        {
            Connectivity.ConnectivityChanged -= OnConnectivityChanged;
        }

        private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs args)
        {
            UpdateLastOnline(args.NetworkAccess);
        }

        private void UpdateLastOnline(NetworkAccess networkAccess)
        {
            var isNetworkAvailable = networkAccess == NetworkAccess.Internet ||
                                     networkAccess == NetworkAccess.ConstrainedInternet;

            if (isNetworkAvailable)
            {
                _dataService.LastOnlineDate = DateTime.UtcNow;
            }

            var lastUpdatedSpan = _dataService.LastJWKSUpdateDate - DateTime.UtcNow;
            var needsJWKSUpdate = lastUpdatedSpan.Value > AppSettings.JWKSUpdateThreshold;
                NetworkConnectivityChanged?.Invoke(this,
                    new NetworkInfo(IsNetworkAvailable, _dataService.LastOnlineDate, _dataService.LastJWKSUpdateDate, needsJWKSUpdate));
        }

    }
    
}
