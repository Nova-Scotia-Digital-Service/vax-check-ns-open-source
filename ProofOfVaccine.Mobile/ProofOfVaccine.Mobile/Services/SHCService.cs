using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProofOfVaccine.Mobile.DTOs;
using Xamarin.Forms;

namespace ProofOfVaccine.Mobile.Services
{
    public interface ISHCService
    {
        SCHData LastScanData { get; }
        SCHData ValidateQRCode(string QRCode);
        SCHData ValidateSCHCode(string SHCCode);
    }

    public class SHCService : ISHCService
    {
        protected readonly IErrorManagementService _errorManagementService;
        public SHCService()
        {
            _errorManagementService = DependencyService.Resolve<IErrorManagementService>();
        }

        public SCHData LastScanData { get; internal set; }

        public SCHData ValidateQRCode(string QRCode)
        {
            try
            {
                var IsSCHCode = string.Equals(QRCode.Substring(0, 3), "shc");

                if (IsSCHCode)
                    return ValidateSCHCode(QRCode);
                else
                    return null;
            }
            catch(Exception ex)
            {
                if (_errorManagementService != null)
                    _errorManagementService.HandleError(ex);
                return null;
            }

        }

        public SCHData ValidateSCHCode(string SHCCode)
        {
            //TODO: Decode SHC code and return data

            try
            {
                return LastScanData = new SCHData()
                {
                    IsValidProof = true,
                    SHCCode = SHCCode,
                    Name = "Jane Doe"
                };
            }
            catch (Exception ex)
            {
                if (_errorManagementService != null)
                    _errorManagementService.HandleError(ex);
                return null;
            }        
        }
    }
}
