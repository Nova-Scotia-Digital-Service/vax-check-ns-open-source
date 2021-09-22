using PoV.Decode.DataStore;
using PoV.Decode.Providers;
using ProofOfVaccine.Decode.Decoder;
using ProofOfVaccine.Mobile.DataStore;
using ProofOfVaccine.Mobile.Services;
using ProofOfVaccine.Token.Model.Jwks;
using ProofOfVaccine.Token.Providers;
﻿using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ProofOfVaccine.Mobile
{
    public partial class App : Application
    {
        private readonly IPersistentJwksProvider<IJwksDataStore> _persistentJwksProvider;
        private bool _jwksLoaded = false;

        //TODO: Remove once we start loading from the resources.
        private readonly List<Uri> _whiteList = new List<Uri>
        {
            new Uri("https://spec.smarthealth.cards/examples/issuer/.well-known/jwks.json"),
        };

        public App()
        {
            InitializeComponent();

            //TODO: Remove once we start loading from the resources.
            var defaultCache = new JwksCache(TimeSpan.FromDays(365));
            defaultCache.Set(
                new Uri("https://spec.smarthealth.cards/examples/issuer/.well-known/jwks.json"),
                new JsonWebKeySet(new List<JsonWebKey>
                    {
                        new JsonWebKey("EC", "3Kfdg-XwP-7gXyywtUfUADwBumDOPKMQx-iELL11W9s", "sig", "ES256", "P-256", "11XvRWy1I2S0EyJlyf_bWfw_TQ5CJJNLw78bHXNxcgw", "eZXwxvO1hvCY0KucrPfKo7yAyMT6Ajc3N7OkAB6VYy8"),
                    }));

            _persistentJwksProvider = new PersistentJwksProvider(new SecureStore(), _whiteList, defaultCache);

            DependencyService.RegisterSingleton<IPersistentJwksProvider<IJwksDataStore>>(_persistentJwksProvider);
            DependencyService.RegisterSingleton<IDecoder>(new PersistentSmartHealthCardDecoder(_persistentJwksProvider));
            DependencyService.RegisterSingleton<IErrorManagementService>(new ErrorManagementService());
            DependencyService.RegisterSingleton<ILocalDataService>(new LocalDataService());
            DependencyService.RegisterSingleton<ICoreService>(new CoreService());
            DependencyService.RegisterSingleton<ISHCService>(new SHCService());

            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            InitializeAppCenter();
            var hasConnectivity = Connectivity.NetworkAccess == NetworkAccess.Internet;
            await Task.Run(async() => await _persistentJwksProvider.TryInitializeJwksAsync(hasConnectivity));
            //Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            await OnConnected(e.NetworkAccess);
        }

        private async Task OnConnected(NetworkAccess networkAccess)
        {
            // if already load, don't load again.
            if (_jwksLoaded)
            {
                return;
            }

            var hasConnectivity = networkAccess == NetworkAccess.Internet;
            if (hasConnectivity)
            {
                await _persistentJwksProvider.TryInitializeJwksAsync(hasConnectivity);
                _jwksLoaded = true;
            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        private void InitializeAppCenter()
        {
            AppCenter.Start("android=c8da9da7-50dd-42a8-b7f4-312370215d5a;" +
                            "ios=b680b5fc-214c-49a1-9bce-5decfe8b445a;",
                            typeof(Analytics), typeof(Crashes));
        }
    }
}
