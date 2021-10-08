using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VaxCheckNS.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            TrackPageLoadedEvent();
        }

        private void TrackPageLoadedEvent()
        {
            Analytics.TrackEvent("Dev: Page Loaded", new Dictionary<string, string>() {{"Page", nameof(HomePage) }});
        }

        private void CrashApp_Clicked(object sender, EventArgs e)
        {
            try
            {
                Crashes.GenerateTestCrash();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
                Analytics.TrackEvent("DEV: Manual Force Crash ");
            }
            
        }

    }
}