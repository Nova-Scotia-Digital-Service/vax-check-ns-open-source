using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VaxCheckNS.Mobile.DTOs;
using Xamarin.Essentials;

namespace VaxCheckNS.Mobile.Services
{
    public interface IErrorManagementService
    {
        void HandleError(Exception ex);
        void BadScan(ProofOfVaccinationData data, string message);
        void ReportEvent(string eventName, string message);
    }

    public class ErrorManagementService : IErrorManagementService
    {

        public ErrorManagementService()
        {

        }

        public void HandleError(Exception ex)
        {
#if RELEASE
            Report(ex);
#endif
        }

        private void Report(Exception ex)
        {
            //Crashes.TrackError(ex);
        }

        private void InternalReport(Exception ex)
        {

        }

        public void BadScan(ProofOfVaccinationData data, string message)
        {

            //Analytics.TrackEvent("Invalid Scan",
            //	  new Dictionary<string, string>
            //	  {
            //		{ "Time", DateTime.UtcNow.ToString() },
            //		{ "Issuer", data.Issuer},
            //		{ "ExceptionMessage", message},
            //		{ "Version", VersionTracking.CurrentVersion},
            //		{ "Platform", DeviceInfo.Platform.ToString()}
            //	  });
        }

        public void ReportEvent(string eventName, string message)
        {
            //Analytics.TrackEvent(eventName,
            //	  new Dictionary<string, string>
            //	  {
            //		{ "Time", DateTime.UtcNow.ToString() },
            //		{ "Message", message},
            //		{ "Version", VersionTracking.CurrentVersion},
            //		{ "Platform", DeviceInfo.Platform.ToString()}
            //	  });
        }
    }
}
