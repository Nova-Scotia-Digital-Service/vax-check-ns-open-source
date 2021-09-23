using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ProofOfVaccine.Decode.Decoder;
using ProofOfVaccine.Mobile.DTOs;
using Xamarin.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms.Internals;

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
            catch (Exception ex)
            {
                if (_errorManagementService != null)
                    _errorManagementService.HandleError(ex);
                return null;
            }

        }

        public async Task<SCHData> ValidateSCHCode(string SHCCode)
        {
            try
            {
                var compactJWS = ShcToCompactJws(SHCCode);
                var smartHealthCardModel = await _decoder.DecodeAsync(compactJWS);
                var FHIRJsonObj = JObject.Parse(smartHealthCardModel.VerifiableCredential.CredentialSubject.FhirBundle);

                var givenName = FHIRJsonObj.SelectToken("$....given").FirstOrDefault().ToString();
                string familyName = FHIRJsonObj.SelectToken("$....family").ToString();
                var birthDate = FHIRJsonObj.SelectToken("$....birthDate").ToString();

                //var givenNameToken = FHIRJsonObj.SelectToken("$....given");
                //if (givenNameToken.HasValues)
                //{
                //    if (givenNameToken.Type == JTokenType.Array)
                //    {
                //        givenName = string.Empty;
                //        foreach (var name in givenNameToken)
                //            givenName += givenNameToken + " ";
                //    }
                //    else
                //    {

                //    }
                //}


                return LastScanData = new SCHData()
                {
                    IsValidProof = IsVaildProof(FHIRJsonObj),
                    SHCCode = SHCCode,
                    GivenName = givenName,
                    FamilyName = familyName,
                    DateOfBirth = birthDate
                };
            }
            catch (Exception ex)
            {
                if (_errorManagementService != null)
                    _errorManagementService.HandleError(ex);
                return null;
            }
        }

        private string ShcToCompactJws(string SHCCode)
        {
            Regex rx = new Regex(@"(\d\d?)");
            MatchCollection digitPairs = rx.Matches(SHCCode);
            return digitPairs
                .Select(m => Convert.ToChar(int.Parse(m.Value) + B64_OFFSET).ToString())
                .Aggregate((chain, block) => $"{chain}{block}");
        }

        private bool IsVaildProof(JObject FIHRJsonObjt)
        {
            //TODO: Retrieve validation rules
            //      Apply rules to data

            return true;
        }
    }
}
