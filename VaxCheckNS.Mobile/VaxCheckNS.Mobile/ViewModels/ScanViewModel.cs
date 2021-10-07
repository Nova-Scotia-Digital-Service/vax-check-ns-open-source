using VaxCheckNS.Mobile.Services;
using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Xamarin.Forms;

namespace VaxCheckNS.Mobile.ViewModels
{
    public class ScanViewModel : BaseViewModel
    {
        private int _timeOutSeconds = 10;
        private Timer _timer;

        private string _countdownText = string.Empty;
        public string CountdownText
        {
            get { return _countdownText; }
            set { SetProperty(ref _countdownText, value); }
        }

        public Command AnalyseScanResultCommand { get; set; }
        public Command LeaveCommand { get; set; }

        protected readonly ISHCService _shcService;
        public ScanViewModel()
        {
            _shcService = DependencyService.Resolve<ISHCService>();

            LeaveCommand = new Command(GoBack);
            AnalyseScanResultCommand = new Command<ZXing.Result>(async result => await AnalyseScanAsync(result));

            StartTimer();
        }

        public override void GoBack()
        {
            _timer.Elapsed -= UpdateCount;

            base.GoBack();
            Shell.Current.FlyoutIsPresented = false;
        }

        public override void BackAndNavigateTo(string page, bool hasAnimation = true)
        {
            _timer.Elapsed -= UpdateCount;
            base.BackAndNavigateTo(page);
        }

        private void StartTimer()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += UpdateCount;
            _timer.Start();
        }

        private void UpdateCount(object sender, ElapsedEventArgs e)
        {
            _timeOutSeconds--;
            CountdownText = _timeOutSeconds.ToString();

            if (_timeOutSeconds <= 0)
                GoBack();
        }

        private async Task AnalyseScanAsync(ZXing.Result result)
        {
            using (Busy())
            {
                try
                {
                    if (result.BarcodeFormat == ZXing.BarcodeFormat.QR_CODE)
                    {
                        var SHCdata = await _shcService.ValidateQRCode(result.Text);

                        if (SHCdata == null) return;

                        BackAndNavigateTo("ScanResultPage");
                    }

                }
                catch (Exception ex)
                {
                    if (_errorManagementService != null)
                        _errorManagementService.HandleError(ex);
                }
            }
        }
    }
}
