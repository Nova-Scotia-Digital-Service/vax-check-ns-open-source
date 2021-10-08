using VaxCheckNS.Mobile.Services;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

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
		{//TODO: replace services with mock
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
			InitializeAppCenter();
			await _shcService.InitializeAsync();
		}

		protected override void OnSleep()
		{
		}

		protected override async void OnResume()
		{
			await _shcService.TryUpdateKeyset();
		}

		private void InitializeAppCenter()
		{
			AppCenter.Start("android=c8da9da7-50dd-42a8-b7f4-312370215d5a;" +
							"ios=b680b5fc-214c-49a1-9bce-5decfe8b445a;",
							typeof(Analytics), typeof(Crashes));
		}
	}
}
