using System;
using System.Collections.Generic;
using System.Text;
using VaxCheckNS.Mobile.Services;
using Xamarin.Forms;

namespace VaxCheckNS.Mobile.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private const double offineThreshold = 7.0;

        private readonly ILocalDataService _localDataService;

        private double _daysOffline;

        public double DaysOffline
        {
            get => _daysOffline;
            set => SetProperty(ref _daysOffline, value);
        }

        public HomeViewModel()
        {
            _localDataService = DependencyService.Resolve<ILocalDataService>();

            _errorManagementService.ReportEvent("Page Initial Open", "Home view launched");
        }
    }
}
