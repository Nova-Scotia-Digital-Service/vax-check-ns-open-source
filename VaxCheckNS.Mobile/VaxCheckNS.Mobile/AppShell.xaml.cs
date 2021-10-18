using VaxCheckNS.Mobile.ViewModels;
using VaxCheckNS.Mobile.Views;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace VaxCheckNS.Mobile
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
            Routing.RegisterRoute(nameof(HomePage)+"/"+nameof(AboutPage), typeof(AboutPage));
            Routing.RegisterRoute(nameof(HomePage) + "/" + nameof(FAQPage), typeof(FAQPage));
        }

    }
}
