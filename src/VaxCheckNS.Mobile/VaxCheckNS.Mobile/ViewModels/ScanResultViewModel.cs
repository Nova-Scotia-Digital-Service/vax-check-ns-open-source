using VaxCheckNS.Mobile.DTOs;
using VaxCheckNS.Mobile.Services;
using Xamarin.Forms;

namespace VaxCheckNS.Mobile.ViewModels
{
    public class ScanResultViewModel : BaseViewModel
    {
        private ProofOfVaccinationData _proofOfVaccine = null;
        public ProofOfVaccinationData ProofOfVaccineData
        {
            get { return _proofOfVaccine; }
            set { SetProperty(ref _proofOfVaccine, value); }
        }

        public Command NewScanCommand { get; set; }

        protected readonly ISHCService _shcService;
        public ScanResultViewModel()
        {
            _shcService = DependencyService.Resolve<ISHCService>();

            NewScanCommand = new Command(() => BackAndNavigateTo("ScanPage"));

            LoadScanData();
        }

        private void LoadScanData()
        {
            ProofOfVaccineData = _shcService.LastScanData;
        }
    }
}
