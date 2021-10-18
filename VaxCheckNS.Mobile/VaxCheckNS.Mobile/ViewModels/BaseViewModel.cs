using VaxCheckNS.Mobile.DTOs;
using VaxCheckNS.Mobile.Helpers;
using VaxCheckNS.Mobile.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace VaxCheckNS.Mobile.ViewModels
{
    public class BaseViewModel : BaseBindable
    {
        private string _title = string.Empty;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private bool _showUpdateMessage = false;
        public bool ShowUpdateMessage
        {
            get { return _showUpdateMessage; }
            set { SetProperty(ref _showUpdateMessage, value); }
        }

        private bool _toUAccepted;
        public bool ToUAccepted
        {
            get { return _toUAccepted; }
            set { SetProperty(ref _toUAccepted, value); }
        }

        private bool _privacyAccepted;
        public bool PrivacyAccepted
        {
            get { return _privacyAccepted; }
            set { SetProperty(ref _privacyAccepted, value); }
        }

        private bool _tutorialComplete;
        public bool TutorialComplete
        {
            get { return _tutorialComplete; }
            set { SetProperty(ref _tutorialComplete, value); }
        }

        public string VersionNumber => VersionTracking.CurrentVersion;
        public NetworkInfo CurrentNetworkInfo { get; private set; }

        public Command GoBackCommand { get; set; }
        public Command<string> GoToCommand { get; set; }
        public Command ScanCommand { get; set; }
        public Command<string> OpenInBroswerCommand { get; set; }
        public Command CloseBannerCommand { get; set; }

        protected readonly IErrorManagementService _errorManagementService;
        protected readonly IConnectivityService _connectivityService;
        protected readonly IDataService _dataService;
        public BaseViewModel()
        {
            _errorManagementService = DependencyService.Resolve<IErrorManagementService>();
            _connectivityService = DependencyService.Resolve<IConnectivityService>();
            _dataService = DependencyService.Resolve<IDataService>();

            GoBackCommand = new Command(GoBack);
            GoToCommand = new Command<string>(pagePath => GoTo(pagePath));
            ScanCommand = new Command(Scan);
            OpenInBroswerCommand = new Command<string>(async url => await OpenInBrowserAsync(new Uri(url)));
            CloseBannerCommand = new Command(() => ShowUpdateMessage = false);

            ToUAccepted = _dataService.VerifyTermsOfUseAccepted();
            PrivacyAccepted = _dataService.VerifyPrivacyAccepted();
            TutorialComplete = _dataService.VerifyTutorialComplete();

            _connectivityService.NetworkConnectivityChanged += OnNetworkConnectivityChanged;
            SetNetworkInfo(_connectivityService.GetNetworkInfo);
        }
        ~BaseViewModel()
        {
            _connectivityService.NetworkConnectivityChanged -= OnNetworkConnectivityChanged;
        }

        private void OnNetworkConnectivityChanged(object sender, NetworkInfo info)
        {
            SetNetworkInfo(info);
        }

        private void SetNetworkInfo(NetworkInfo info)
        {
            CurrentNetworkInfo = info;
            RaisePropertyChanged(nameof(CurrentNetworkInfo));

            ShowUpdateMessage = info.IsJWKSOutdated;
        }

        public async Task OpenInBrowserAsync(Uri uri)
        {
            try
            {
                await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                _errorManagementService.HandleError(ex);
            }
        }

        public virtual void Scan()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                using (Busy())
                    await Shell.Current.GoToAsync("ScanPage", true);
            });
        }
        public virtual void GoBack()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                using (Busy())
                    await Shell.Current.GoToAsync("..", true);
            });
        }
        public virtual void GoTo(string pagePath, bool hasAnimation = true)
        {

            Device.BeginInvokeOnMainThread(async () =>
            {
                IsBusy = true;
                //using (Busy())
                //{
                Shell.Current.FlyoutIsPresented = false;
                await Shell.Current.GoToAsync(pagePath, hasAnimation);
                //}
                IsBusy = false;

            });
        }
        public virtual void BackAndNavigateTo(string page, bool hasAnimation = true)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                //using (Busy())
                IsBusy = true;
                await Shell.Current.GoToAsync("../" + page, hasAnimation);
                IsBusy = false;
            });
        }

        #region Busy Mechanism
        private static readonly Guid _defaultTracker = new Guid(AppSettings.AppPrivateKey);
        private IList<Guid> _busyLocks = new List<Guid>();

        public bool IsBusy
        {
            get => _busyLocks.Any();
            set
            {
                if (value && !_busyLocks.Contains(_defaultTracker))
                {
                    _busyLocks.Add(_defaultTracker);
                    RaisePropertyChanged(nameof(IsBusy));
                }

                if (!value && _busyLocks.Contains(_defaultTracker))
                {
                    _busyLocks.Remove(_defaultTracker);
                    RaisePropertyChanged(nameof(IsBusy));
                }
            }
        }

        protected BusyHelper Busy() => new BusyHelper(this);

        protected void ForceUnlock()
        {
            _busyLocks = new List<Guid>();
        }

        protected bool IsThreadLocked = false;

        protected sealed class BusyHelper : IDisposable
        {
            private readonly BaseViewModel _viewModel;
            private readonly Guid _tracker;

            public BusyHelper(BaseViewModel viewModel)
            {
                _viewModel = viewModel;
                _tracker = Guid.NewGuid();
                _viewModel._busyLocks.Add(_tracker);
                _viewModel.RaisePropertyChanged(nameof(IsBusy));
            }

            public void Dispose()
            {
                _viewModel._busyLocks.Remove(_tracker);
                _viewModel.RaisePropertyChanged(nameof(IsBusy));
            }
        }
        #endregion
    }
}
