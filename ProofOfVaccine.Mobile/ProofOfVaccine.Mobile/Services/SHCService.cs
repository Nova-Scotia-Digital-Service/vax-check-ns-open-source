using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ProofOfVaccine.Decode.Decoder;
using ProofOfVaccine.Mobile.DTOs;
using Xamarin.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ProofOfVaccine.Mobile.Services
{
    public interface ISHCService
    {
        SCHData LastScanData { get; }
        Task<SCHData> ValidateQRCode(string QRCode);
        Task<SCHData> ValidateSCHCode(string SHCCode);
    }

    public class SHCService : ISHCService
    {
        private const int B64_OFFSET = 45;

        protected readonly IErrorManagementService _errorManagementService;
        protected readonly IDecoder _decoder;
        public SHCService()
        {
            _errorManagementService = DependencyService.Resolve<IErrorManagementService>();
            _decoder = DependencyService.Resolve<IDecoder>();
        }

        public SCHData LastScanData { get; internal set; }

        public async Task<SCHData> ValidateQRCode(string QRCode)
        {
            try
            {
                var IsSCHCode = string.Equals(QRCode.Substring(0, 3), "shc");

                if (IsSCHCode)
                    return await ValidateSCHCode(QRCode);
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

        public async Task<SCHData> ValidateSCHCode(string SHCCode)
        {
            //TODO: Decode SHC code and return data

            try
            {
                var compactJWS = ShcToCompactJws(SHCCode);
                var smartHealthCardModel = await _decoder.DecodeAsync(compactJWS);
                var fhirJsonObj = JObject.Parse(smartHealthCardModel.VerifiableCredential.CredentialSubject.FhirBundle);
                string familyName = fhirJsonObj.SelectToken("$....family").ToString();
                var givenName = fhirJsonObj.SelectToken("$....given").FirstOrDefault().ToString();

                return LastScanData = new SCHData()
                {
                    IsValidProof = true,
                    SHCCode = SHCCode,
                    Name = $"{givenName} {familyName}"
                };
            }
            catch (Exception ex)
            {
                if (_errorManagementService != null)
                    _errorManagementService.HandleError(ex);
                return null;
            }        
        }

        /// <summary>
        /// Converts the SHC code to the compact JWS format.
        /// </summary>
        /// <param name="SHCCode"></param>
        /// <returns></returns>
        private string ShcToCompactJws(string SHCCode)
        {
            Regex rx = new Regex(@"(\d\d?)");
            MatchCollection digitPairs = rx.Matches(SHCCode);
            return digitPairs
                .Select(m => Convert.ToChar(int.Parse(m.Value) + B64_OFFSET).ToString())
                .Aggregate((chain, block) => $"{chain}{block}");
        }
    }
}
