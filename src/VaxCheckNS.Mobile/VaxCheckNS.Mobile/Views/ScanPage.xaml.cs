using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VaxCheckNS.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanPage : ContentPage
    {
        public ScanPage()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (height < width)
            {
                Guide.HeightRequest = (int)Math.Truncate(Application.Current.MainPage.Height / 2) + 20;
                Guide.WidthRequest = (int)Math.Truncate(Application.Current.MainPage.Height / 2);
            }
            else
            {
                Guide.HeightRequest = (int)Math.Truncate(Application.Current.MainPage.Width / 2) + 20;
                Guide.WidthRequest = (int)Math.Truncate(Application.Current.MainPage.Width / 2);
            } 
        }

        protected override async void OnDisappearing()
        {
            // BUG: Ending the scanning early seems to help with the animation issues. 
            if (Device.RuntimePlatform == Device.Android)
            {
                scanner.IsScanning = false;
            }
            base.OnDisappearing();

            // BUG: For iOS, scanning needs to continue to prevent screen flashing until after animation.
            // Must be turned off after. It appears that not turning it off causes issues with hanging.
            await Task.Delay(150);
            scanner.IsScanning = false;
        }


        protected override bool OnBackButtonPressed()
        {
            vm.LeaveCommand.Execute(null);
            return true;
        }
    }
}