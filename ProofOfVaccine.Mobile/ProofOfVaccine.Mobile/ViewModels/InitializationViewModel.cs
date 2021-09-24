using System;
using System.Threading.Tasks;
using ProofOfVaccine.Mobile.Services;
using Xamarin.Forms;

namespace ProofOfVaccine.Mobile.ViewModels
{
    public class InitializationViewModel : BaseViewModel
    {
        private readonly ISHCService _shcService;

        public InitializationViewModel()
        {
            _shcService = DependencyService.Resolve<ISHCService>();
        }

        public async Task InitializeAsync()
        {
            await _shcService.InitializeAsync();
        }
    }
}
