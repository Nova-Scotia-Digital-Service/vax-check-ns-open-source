﻿using ProofOfVaccine.Mobile.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

        protected readonly ISHCService _SHCService;
        public ScanViewModel()
        {

            _SHCService = DependencyService.Resolve<ISHCService>();

            AnalyseScanResultCommand = new Command<ZXing.Result>(async result => await AnalyseScan(result));
        }

        private async Task AnalyseScan(ZXing.Result result)
        {
            if (!IsBusy)
                using (Busy())
                {
                    try
                    {
                        if (result.BarcodeFormat == ZXing.BarcodeFormat.QR_CODE)
                        {
                            var SHCdata = _SHCService.ValidateQRCode(result.Text);

                            if (SHCdata == null)
                            {

                                Device.BeginInvokeOnMainThread(async () =>
                                {
                                    using (Busy())
                                        await Application.Current.MainPage.DisplayAlert("Scan Error", "Not a valid QR Code", "Try Again");
                                });

                            }
                            else
                            {
                                Device.BeginInvokeOnMainThread(async () =>
                                {
                                    using (Busy())
                                        await Shell.Current.GoToAsync("../ScanResultPage");
                                });
                            }
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
