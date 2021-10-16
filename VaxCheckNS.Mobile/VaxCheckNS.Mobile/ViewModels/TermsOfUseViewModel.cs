using System;
using VaxCheckNS.Mobile.Services;
using Xamarin.Forms;

namespace VaxCheckNS.Mobile.ViewModels
{
    public class TermsOfUseDeclinedException : Exception
    {
        public TermsOfUseDeclinedException() : base(message: "Terms of use was declined by user.")
        {
        }
    }

    public class TermsOfUseViewModel : BaseViewModel
    {
        private new readonly IDataService _dataService;
        public Command AcceptCommand { get; set; }
        public Command DeclineCommand { get; set; }

        public TermsOfUseViewModel()
        {
            _dataService = DependencyService.Resolve<IDataService>();
            AcceptCommand = new Command(() => Accept());
            DeclineCommand = new Command(() => Decline());
        }

        public void Accept()
        {
            _dataService.AcceptTermsOfUse();
            GoTo("///PrivacyPage");
        }

        public void Decline()
        {
            throw new TermsOfUseDeclinedException();
        }
    }
}
