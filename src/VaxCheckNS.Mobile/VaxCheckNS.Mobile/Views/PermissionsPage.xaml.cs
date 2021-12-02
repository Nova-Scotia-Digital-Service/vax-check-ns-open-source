using System;
using System.Collections.Generic;
using VaxCheckNS.Mobile.ViewModels;
using Xamarin.Forms;

namespace VaxCheckNS.Mobile.Views
{
    public partial class PermissionsPage : ContentPage
    {
        public PermissionsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ((PermissionsViewModel)BindingContext).Initialize();
        }
    }
}
