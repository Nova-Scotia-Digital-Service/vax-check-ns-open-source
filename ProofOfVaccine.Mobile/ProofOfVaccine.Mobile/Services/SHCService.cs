using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ProofOfVaccine.Decode.Decoder;
using ProofOfVaccine.Mobile.DTOs;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using PoV.Decode.Providers;
using PoV.Decode.DataStore;
using System.Collections.Generic;
using ProofOfVaccine.Token.Model.Jwks;
using ProofOfVaccine.Token.Providers;
using ProofOfVaccine.Mobile.DataStore;
using ProofOfVaccine.Rules.Validator;
using ProofOfVaccine.Mobile.AppResources;
using ProofOfVaccine.Token.Exceptions;

namespace ProofOfVaccine.Mobile.Services
{
    public interface ISHCService
    {
        Task InitializeAsync();
        Task<ProofOfVaccinationData> ValidateQRCode(string QRCode);
        Task<ProofOfVaccinationData> ValidateVaccination(string SHCCode);
        ProofOfVaccinationData LastScanData { get; }
        ProofOfVaccinationData InvaidScan(string message, string code);
    }

    public class SHCService : ISHCService
    {
        private const int B64_OFFSET = 45;

        private readonly string _invalidVaccineCodeResource = TextResources.VaccineCodeInvalidText;
        private readonly string _invalidVaccineDateResource = TextResources.VaccineDateInvalidText;
        private readonly string _invalidVaccineDosageResource = TextResources.VaccineDosageInvalidText;
        private readonly string _invalidFhirFormatResource = TextResources.FhirFormatInvalidText;
        private readonly string _invalidScanResource = TextResources.InvalidScanText;

        protected readonly IErrorManagementService _errorManagementService;
        protected readonly IDecoder _decoder;
        private readonly IPersistentJwksProvider<IJwksDataStore> _persistentJwksProvider;

        //TODO: Remove once we start loading from the resources.
        private readonly List<Uri> _whiteList = new List<Uri>
        {
            new Uri("https://sync-cf2-1.qa.canimmunize.ca/.well-known/jwks.json"),
            new Uri("https://spec.smarthealth.cards/examples/issuer/.well-known/jwks.json"),
            new Uri("https://pvc-dev.novascotia.ca/issuer/.well-known/jwks.json"),
            // If we connect to the internet, we can scan the examples on the smart health card website.
            // Not included in the default data.
        };

        //TODO: Remove once we start loading from the resources.
        private readonly IList<string> _validVaccineCodes= new List<string>
        {
            "207", //Moderna
            "208", // Pfizer
            "210", // AZ
            "212"  // J&J
        };

        //TODO: Remove once we start loading from the resources.
        private readonly IList<string> _singleDosageVaccineCodes = new List<string>
        {
            "212", // J&J
        };

        private bool _jwksLoaded = false;

        public SHCService()
        {
            //TODO: Remove once we start loading from the resources.
            var defaultCache = new JwksCache(TimeSpan.FromDays(365));
            defaultCache.Set(
                new Uri("https://sync-cf2-1.qa.canimmunize.ca/.well-known/jwks.json"),
                new JsonWebKeySet(new List<JsonWebKey>
                    {
                        new JsonWebKey("EC", "rgJPTaK1nTu897OmUz3oPoeyeReXRMHakf1Dtruu6Z0", "sig", "ES256", "P-256", "ZxJq20yzaU-5-aZZ18FxsVvmlit8DkKnb6UDIIFkWi0", "Oh_I2bHBK4Ex8BOKS1Iw0vYiO1Zf9E3-RZlA9T0r2Lw"),
                    }));

            _errorManagementService = DependencyService.Resolve<IErrorManagementService>();
            _persistentJwksProvider = new PersistentJwksProvider(new SecureStore(), _whiteList, defaultCache);
            _decoder = new PersistentSmartHealthCardDecoder(_persistentJwksProvider);
        }

        public ProofOfVaccinationData LastScanData { get; internal set; }

        public async Task InitializeAsync()
        {
            var hasConnectivity = Connectivity.NetworkAccess == NetworkAccess.Internet;
            await Task.Run(async () => await _persistentJwksProvider.TryInitializeJwksAsync(hasConnectivity));
        }

        public async Task<ProofOfVaccinationData> ValidateQRCode(string QRCode)
        {
            try
            {
                var IsSCHCode = string.Equals(QRCode.Substring(0, 3), "shc");

                if (IsSCHCode)
                    return await ValidateVaccination(QRCode);
                else
                {
                    return InvaidScan(_invalidScanResource, "400");
                }
            }
            catch (Exception ex)
            {
                if (_errorManagementService != null)
                    _errorManagementService.HandleError(ex);
                return null;
            }

        }

        public ProofOfVaccinationData InvaidScan(string message, string code)
        {
            return LastScanData = new ProofOfVaccinationData()
            {
                IsValidProof = false,
                Code = code,
                Message = message
            };
        }

        public async Task<ProofOfVaccinationData> ValidateVaccination(string SHCCode)
        {
            try
            {
                var compactJWS = ShcToCompactJws(SHCCode);
                var smartHealthCardModel = await _decoder.DecodeAsync(compactJWS);
                var fhirBundle = JObject.Parse(smartHealthCardModel.VerifiableCredential.CredentialSubject.FhirBundle);
                return CreateProofOfVaccineData(fhirBundle);
            }
            catch (Exception ex)
            {
                if (_errorManagementService != null)
                    _errorManagementService.HandleError(ex);
                if (ex is SmartHealthCardDecoderException)
                {
                    return InvaidScan(_invalidScanResource, "400");
                }
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

        private ProofOfVaccinationData CreateProofOfVaccineData(JObject fhir)
        {
            var givenName = fhir
                .SelectToken("$....given")
                .FirstOrDefault()
                .ToString();
            string familyName = fhir
                .SelectToken("$....family")
                .ToString();
            var birthDate = fhir
                .SelectToken("$....birthDate")
                .ToString();

            using (var vaccineValidator = new NSVaccineValidator(fhir,
                _validVaccineCodes, _singleDosageVaccineCodes,
                _invalidVaccineCodeResource, _invalidVaccineDateResource,
                _invalidVaccineDosageResource, _invalidFhirFormatResource))
            {
                var result = vaccineValidator.Validate();
                return LastScanData = new ProofOfVaccinationData()
                {
                    IsValidProof = result.Success,
                    Message = result.Message,
                    GivenName = givenName,
                    FamilyName = familyName,
                    DateOfBirth = birthDate,
                    Code = result.Success ? "200" : "400"
                };
            }
        }
    }
}
