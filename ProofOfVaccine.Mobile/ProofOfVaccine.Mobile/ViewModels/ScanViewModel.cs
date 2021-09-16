using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace ProofOfVaccine.Mobile.ViewModels
{
    public class ScanViewModel : BaseViewModel
    {
        ZXing.Result _result = null;
        public ZXing.Result ScanResult
        {
            get { return _result; }
            set
            {
                if (value != null)
                {
                    AnalyseScanResultCommand.Execute(value);
                    SetProperty(ref _result, value);
                }
            }
        }

        public ICommand AnalyseScanResultCommand;

        public ScanViewModel()
        {
            AnalyseScanResultCommand = new Command<ZXing.Result>(async result =>
            {
                if (!IsBusy)
                    using (Busy())
                    {
                        //TODO: Save Result in a service (save on RAM),
                        //use SHCService to analyse and return result,
                        //Based on result, navigate to ScanResultPage


                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Shell.Current.GoToAsync("../ScanResultPage");
                        });
                    }

            });
        }
    }
}
