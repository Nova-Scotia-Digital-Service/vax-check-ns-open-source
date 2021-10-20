using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                var requestedStatus = await Permissions.RequestAsync<Permissions.Camera>();
                if (requestedStatus != PermissionStatus.Granted)
                {
                    vm.LeaveCommand.Execute(null);
                }
            }

        }

        protected override bool OnBackButtonPressed()
        {
            vm.LeaveCommand.Execute(null);
            return true;
        }
    }
}