using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProofOfVaccine.Mobile.Animations;
using ProofOfVaccine.Mobile.ViewModels;
using Xamarin.Forms;

namespace ProofOfVaccine.Mobile.Views
{
    public partial class InitializationPage : ContentPage
    {
        public InitializationPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await vm.InitializeAsync();
            await Shell.Current.GoToAsync("//HomePage");
            base.OnAppearing();
        }
    }
}
