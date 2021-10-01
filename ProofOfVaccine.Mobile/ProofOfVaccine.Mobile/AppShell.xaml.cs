using ProofOfVaccine.Mobile.ViewModels;
using ProofOfVaccine.Mobile.Views;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ProofOfVaccine.Mobile
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();

            RegisterPages();
        }

        
        private void RegisterPages()
        {
            Routing.RegisterRoute(nameof(LoadingPage), typeof(LoadingPage));
            Routing.RegisterRoute(nameof(ScanPage), typeof(ScanPage));
            Routing.RegisterRoute(nameof(ScanResultPage), typeof(ScanResultPage));
            Routing.RegisterRoute(nameof(TutorialPage), typeof(TutorialPage));
            Routing.RegisterRoute(nameof(HomePage)+"/"+nameof(AboutPage), typeof(AboutPage));
        }

    }
}
