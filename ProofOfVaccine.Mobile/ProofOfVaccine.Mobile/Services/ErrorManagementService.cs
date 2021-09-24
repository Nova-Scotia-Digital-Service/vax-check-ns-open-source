using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProofOfVaccine.Mobile.Services
{
    public interface IErrorManagementService
    {
        void HandleError(Exception ex);
    }

    public class ErrorManagementService : IErrorManagementService
    {

        public ErrorManagementService()
        {

        }

        public void HandleError(Exception ex)
        {
#if !DEBUG
            Report(ex);
#endif
        }

        private void Report(Exception ex)
        {
            Crashes.TrackError(ex);
        }

        private void InternalReport(Exception ex)
        {

        }
    }
}
