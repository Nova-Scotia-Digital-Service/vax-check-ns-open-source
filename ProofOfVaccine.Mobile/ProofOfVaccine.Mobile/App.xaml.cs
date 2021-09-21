using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;

using ProofOfVaccine.Mobile.Services;

using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProofOfVaccine.Mobile
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.RegisterSingleton<IErrorManagementService>(new ErrorManagementService());
            DependencyService.RegisterSingleton<ILocalDataService>(new LocalDataService());
            DependencyService.RegisterSingleton<ICoreService>(new CoreService());
            DependencyService.RegisterSingleton<ISHCService>(new SHCService());

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            InitializeAppCenter();
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
                            typeof(Analytics), typeof(Crashes), typeof(Distribute));
        }
    }
}
