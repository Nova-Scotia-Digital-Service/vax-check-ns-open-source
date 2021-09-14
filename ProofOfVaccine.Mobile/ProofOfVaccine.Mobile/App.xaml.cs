using ProofOfVaccine.Mobile.Services;
using ProofOfVaccine.Mobile.Views;
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

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
