using GoogleVisionBarCodeScanner;
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
            GoogleVisionBarCodeScanner.Methods.SetSupportBarcodeFormat(BarcodeFormats.QRCode);
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

        //protected override async void OnDisappearing()
        //{
        //    // BUG: Ending the scanning early seems to help with the animation issues. 
        //    if (Device.RuntimePlatform == Device.Android)
        //    {
        //        scanner.IsScanning = false;
        //    }
        //    base.OnDisappearing();

        //    // BUG: For iOS, scanning needs to continue to prevent screen flashing until after animation.
        //    // Must be turned off after. It appears that not turning it off causes issues with hanging.
        //    await Task.Delay(250);
        //    scanner.IsScanning = false;
        //}

        private async void CameraView_OnDetected(object sender, GoogleVisionBarCodeScanner.OnDetectedEventArg e)
        {
            List<GoogleVisionBarCodeScanner.BarcodeResult> obj = e.BarcodeResults;

            string result = string.Empty;
            for (int i = 0; i < obj.Count; i++)
            {
                result += $"{i + 1}. Type : {obj[i].BarcodeType}, Value : {obj[i].DisplayValue}{Environment.NewLine}";
            }
            Device.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Result", result, "OK");
                //If you want to stop scanning, you can close the scanning page
                await Navigation.PopModalAsync();
                //if you want to keep scanning the next barcode, do not close the scanning page and call below function
                //GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
            });

        }

        protected override bool OnBackButtonPressed()
        {
            vm.LeaveCommand.Execute(null);
            return true;
        }
    }
}