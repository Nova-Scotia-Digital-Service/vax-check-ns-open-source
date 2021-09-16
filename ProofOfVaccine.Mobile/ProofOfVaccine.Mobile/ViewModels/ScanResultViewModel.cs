using ProofOfVaccine.Mobile.DTOs;
using ProofOfVaccine.Mobile.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ProofOfVaccine.Mobile.ViewModels
{
    public class ScanResultViewModel : BaseViewModel
    {
        private SCHData _shcData = null;
        public SCHData SCHValidationData
        {
            get { return _shcData; }
            set { SetProperty(ref _shcData, value); }
        }

        public Command NewScanCommand { get; set; }

        protected readonly ISHCService _shcService;
        public ScanResultViewModel()
        {
            _shcService = DependencyService.Resolve<ISHCService>();

            NewScanCommand = new Command(async () => await Shell.Current.GoToAsync("../ScanPage", true));

            LoadScanData();
        }

        private void LoadScanData()
        {
            SCHValidationData = _shcService.LastScanData;
        }
    }
}
