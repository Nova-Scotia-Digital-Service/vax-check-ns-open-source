using System;
using VaxCheckNS.Mobile.Services;
using Xamarin.Forms;

namespace VaxCheckNS.Mobile.ViewModels
{
    public class TermsOfUseViewModel : BaseViewModel
    {
        private new readonly IDataService _dataService;
        public Command AcceptCommand { get; set; }

        public TermsOfUseViewModel()
        {
            _dataService = DependencyService.Resolve<IDataService>();
            AcceptCommand = new Command(() => Accept());
        }

        public void Accept()
        {
            _dataService.AcceptTermsOfUse();
            GoTo("///PrivacyPage");
        }
    }
}
