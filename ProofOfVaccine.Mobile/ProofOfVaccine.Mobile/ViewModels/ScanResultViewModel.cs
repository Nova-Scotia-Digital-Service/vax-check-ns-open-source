using ProofOfVaccine.Mobile.DTOs;
using ProofOfVaccine.Mobile.Services;
using Xamarin.Forms;

namespace ProofOfVaccine.Mobile.ViewModels
{
    public class ScanResultViewModel : BaseViewModel
    {
        private ProofOfVaccinationModel _proofOfVaccine = null;
        public ProofOfVaccinationModel ProofOfVaccineData
        {
            get { return _proofOfVaccine; }
            set { SetProperty(ref _proofOfVaccine, value); }
        }

        public Command NewScanCommand { get; set; }

        protected readonly ISHCService _shcService;
        public ScanResultViewModel()
        {
            _shcService = DependencyService.Resolve<ISHCService>();

            NewScanCommand = new Command(async () => await Shell.Current.GoToAsync("//HomePage/ScanPage", true));

            LoadScanData();
        }

        private void LoadScanData()
        {
            ProofOfVaccineData = _shcService.LastScanData;
        }
    }
}
