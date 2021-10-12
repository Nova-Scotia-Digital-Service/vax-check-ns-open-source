using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VaxCheckNS.Decode.Decoder;
using VaxCheckNS.Mobile.DTOs;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using VaxCheckNS.Decode.Providers;
using VaxCheckNS.Decode.DataStore;
using System.Collections.Generic;
using VaxCheckNS.Token.Providers;
using VaxCheckNS.Token.Exceptions;
using VaxCheckNS.Rules.NS.Validator;
using VaxCheckNS.Rules.NS.Models;
using Newtonsoft.Json;
using System.Reflection;
using System.IO;
using VaxCheckNS.Token.Model.Jwks;

namespace VaxCheckNS.Mobile.Services
{
    public interface ISHCService
    {
        Task InitializeAsync();
        Task TryUpdateKeyset();
        Task<ProofOfVaccinationData> ValidateQRCode(string QRCode);
        Task<ProofOfVaccinationData> ValidateVaccination(string SHCCode);
        ProofOfVaccinationData LastScanData { get; }
        ProofOfVaccinationData InvalidScan(VaccineStatus? code);
    }

    public class SHCService : ISHCService
    {
        private const int B64_OFFSET = 45;

        protected readonly IErrorManagementService _errorManagementService;
        protected readonly IDecoder _decoder;
        private readonly IPersistentJwksProvider<IJwksDataStore> _persistentJwksProvider;
        private readonly ILocalDataService _localDataService;

        private Dictionary<Uri, Token.Model.Jwks.JsonWebKeySet> _whiteListedJwks;
        private IList<ValidVaccine> _validVaccines;
        private JwksCache _defaultCache;

        public SHCService()
        {
            //TODO: Load all data via DataService
            LoadEmbeddedData();
            _errorManagementService = DependencyService.Resolve<IErrorManagementService>();
            _localDataService = DependencyService.Resolve<ILocalDataService>();
            _persistentJwksProvider = new PersistentJwksProvider(_localDataService, _whiteListedJwks.Keys.ToList(), _defaultCache);
            _decoder = new PersistentSmartHealthCardDecoder(_persistentJwksProvider);
        }

        public ProofOfVaccinationData LastScanData { get; internal set; }

        public async Task InitializeAsync()
        {
            var hasConnectivity = Connectivity.NetworkAccess == NetworkAccess.Internet;
            if (hasConnectivity)
            {
                _localDataService.SetLastOnlineDate();
            }
            await Task.Run(async () => await _persistentJwksProvider.TryInitializeJwksAsync(hasConnectivity));

            //TODO: Implement logic

            // Load whitelist of issuer and keys from json resource (embedded) 
            // Check network connection
            // if online, get JWkeyset for whitelisted issuers and locally store via DataService
            // else -> Load Previously Stored JWKeyset
            // if no previous stored JWKeyset -> Load JWKetset from json resource (embedded) 
            // initillize SmartHealthCardDecoder with new JWKeyset
        }

        public async Task TryUpdateKeyset()
        {
            //TODO: Implement logic

            // Load whitelist of issuer and keys from RAM (should be handdled in DataService, so load from DataService)
            // Check network connection
            // if online, get JWkeyset for whitelisted issuers
            // Load previously stored JWKeyset from disk (should be handdled in DataService, so load from DataService)
            // check if KID matches for each issuer 
            // if the two KIDs does not match
            // Locally store new KID
            // re-initillize SmartHealthCardDecoder with new JWKeyset

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
                    return InvalidScan();
                }
            }
            catch (Exception ex)
            {
                if (_errorManagementService != null)
                    _errorManagementService.HandleError(ex);
                return null;
            }

        }

        public ProofOfVaccinationData InvalidScan(VaccineStatus? code = null)
        {
            return LastScanData = new ProofOfVaccinationData()
            {
                IsValidProof = false,
                Code = code == null ? VaccineStatus.InvalidFormat : code.Value,
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
                    return InvalidScan();
                }
                else if(ex is SmartHealthCardException)
                {
                    return InvalidScan(VaccineStatus.InvalidIssuer);
                }
                else if (ex is SmartHealthCardPayloadException)
                {
                    return InvalidScan(VaccineStatus.InvalidIssuer);
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
                ?.SelectToken("$....given")
                ?.FirstOrDefault()
                ?.ToString();
            string familyName = fhir
                ?.SelectToken("$....family")
                ?.ToString();
            var birthDate = fhir
                ?.SelectToken("$....birthDate")
                ?.ToString();

            using (var vaccineValidator = new NSVaccineValidator(fhir, _validVaccines))
            {
                var result = vaccineValidator.Validate();
                return LastScanData = new ProofOfVaccinationData()
                {
                    IsValidProof = result.Success,
                    GivenName = givenName,
                    FamilyName = familyName,
                    DateOfBirth = birthDate,
                    Code = result.Message
                };
            }
        }

        private void LoadEmbeddedData()
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(SHCService)).Assembly;
            _whiteListedJwks = LoadWhiteListedJwks(assembly);
            _validVaccines = LoadValidVaccines(assembly);
            
            CacheWhitelistJWKS(_whiteListedJwks);
        }

        private IList<ValidVaccine> LoadValidVaccines(Assembly assembly)
        {
            Stream stream = assembly.GetManifestResourceStream("VaxCheckNS.Mobile.AppResources.ValidVaccines.json");

            using (var reader = new StreamReader(stream))
            {
                _validVaccines = JsonConvert.DeserializeObject<List<ValidVaccine>>(reader.ReadToEnd());
            }

            return _validVaccines;
        }

        private Dictionary<Uri, JsonWebKeySet> LoadWhiteListedJwks(Assembly assembly)
        {
            Stream stream = assembly.GetManifestResourceStream("VaxCheckNS.Mobile.AppResources.WhiteList.json");

            using (var reader = new StreamReader(stream))
            {
                _whiteListedJwks = JsonConvert.DeserializeObject<Dictionary<Uri, Token.Model.Jwks.JsonWebKeySet>>(reader.ReadToEnd());
            }

            return _whiteListedJwks;
        }

        private JwksCache CacheWhitelistJWKS(Dictionary<Uri, JsonWebKeySet>  whiteListedJwks)
        {

            _defaultCache = new JwksCache(TimeSpan.MaxValue);

            foreach (var set in whiteListedJwks)
            {
                _defaultCache.Set(set.Key, set.Value);
            }

            return _defaultCache;
        }

    }
}
