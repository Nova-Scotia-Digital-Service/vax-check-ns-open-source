using VaxCheckNS.Mobile.DTOs;
using VaxCheckNS.Mobile.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace VaxCheckNS.Mobile.Services
{
	public interface IConnectivityService
	{
		event ConnectivityService.NetworkConnectivityEventHandler NetworkConnectivityChanged;
		NetworkInfo GetNetworkInfo { get; }
		bool ListenForConnectivityChange { get; set; }

		void TryUpdateLastOnline(bool needsJWKSUpdate);
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

			TryUpdateLastOnline();
		}

		public void DisableConnectivityUpdates()
		{
			Connectivity.ConnectivityChanged -= OnConnectivityChanged;
		}

		private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs args)
		{
			if (IsNetworkAvailable)
				TryUpdateLastOnline(true);
		}

		public void TryUpdateLastOnline(bool JWKSUpdateRequired = false)
		{
			if (IsNetworkAvailable)
			{
				_dataService.LastOnlineDate = DateTime.UtcNow;
			}

			NetworkConnectivityChanged?.Invoke(this,
				new NetworkInfo(IsNetworkAvailable, _dataService.LastOnlineDate, _dataService.LastJWKSUpdateDate, JWKSUpdateRequired));
		}

	}

}
