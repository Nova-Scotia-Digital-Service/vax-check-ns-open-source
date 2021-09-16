using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProofOfVaccine.Mobile.DTOs;

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
        public SCHData LastScanData { get; internal set; }

        public SCHData ValidateQRCode(string QRCode)
        {
            var IsSCHCode = string.Equals(QRCode.Substring(0,3), "shc");

            if (IsSCHCode)
                return ValidateSCHCode(QRCode);
            else
                return null;
        }

        public SCHData ValidateSCHCode(string SHCCode)
        {
            //TODO: Decode SHC code and return data

            return LastScanData = new SCHData()
            {
                IsValidProof = true,
                SHCCode = SHCCode,
                Name = "Jane Doe"
            };
        }
    }
}
