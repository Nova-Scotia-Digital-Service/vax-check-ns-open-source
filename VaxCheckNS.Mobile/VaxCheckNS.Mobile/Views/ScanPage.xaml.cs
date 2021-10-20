using System;
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

        protected override bool OnBackButtonPressed()
        {
            vm.LeaveCommand.Execute(null);
            return true;
        }
    }
}