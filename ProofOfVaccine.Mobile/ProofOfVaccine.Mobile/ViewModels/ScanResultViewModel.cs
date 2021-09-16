using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ProofOfVaccine.Mobile.ViewModels
{
    public class ScanResultViewModel : BaseViewModel
    {
        public Command NewScanCommand { get; set; }

        public ScanResultViewModel()
        {
            NewScanCommand = new Command(async () => await Shell.Current.GoToAsync("../ScanPage", true));
        }
    }
}
