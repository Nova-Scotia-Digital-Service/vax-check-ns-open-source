using VaxCheckNS.Mobile.Services;
using Xamarin.Forms;
using VaxCheckNS.Mobile.Helpers;
using System;

namespace VaxCheckNS.Mobile
{
    public partial class App : Application
    {
        private ISHCService _shcService;

        public App()
        {
            InitializeComponent();

#if ENABLE_TEST_CLOUD
			RegisterMockServices();
#else
            RegisterServices();
#endif

            MainPage = new AppShell();
        }

        private void RegisterServices()
        {
            DependencyService.RegisterSingleton<IErrorManagementService>(new ErrorManagementService());
            DependencyService.RegisterSingleton<IDataService>(new DataService());
            DependencyService.RegisterSingleton<IConnectivityService>(new ConnectivityService());
            //TODO: integrate into DataService and Remove 
            DependencyService.RegisterSingleton<ILocalDataService>(new LocalDataService());

            _shcService = new SHCService();
            DependencyService.RegisterSingleton(_shcService);
        }

        private void RegisterMockServices()
        {//TODO: replace services with mock service implementations
            DependencyService.RegisterSingleton<IErrorManagementService>(new ErrorManagementService());
            DependencyService.RegisterSingleton<IDataService>(new DataService());
            DependencyService.RegisterSingleton<IConnectivityService>(new ConnectivityService());
            //TODO: integrate into DataService and Remove 
            DependencyService.RegisterSingleton<ILocalDataService>(new LocalDataService());

            _shcService = new SHCService();
            DependencyService.RegisterSingleton(_shcService);
        }

        protected override async void OnStart()
        {
            await _shcService.InitializeAsync();
        }

        protected override void OnSleep()
        {
        }

        protected override async void OnResume()
        {
            await _shcService.TryUpdateKeyset();
        }


    }
}
