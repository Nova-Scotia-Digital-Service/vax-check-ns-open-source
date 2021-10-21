using System;
using VaxCheckNS.Mobile.Services;
using Xamarin.Forms;

namespace VaxCheckNS.Mobile.ViewModels
{
    public class PrivacyViewModel : BaseViewModel
    {
        private new readonly IDataService _dataService;
        public Command AcceptCommand { get; set; }
        public Command DeclineCommand { get; set; }

        public PrivacyViewModel()
        {
            _dataService = DependencyService.Resolve<IDataService>();
            AcceptCommand = new Command(() => Accept());
        }

        public void Accept()
        {
            _dataService.AcceptPrivacy();
            //GoTo("///TutorialPage");
            GoTo("///PermissionsPage");
        }
    }
}
