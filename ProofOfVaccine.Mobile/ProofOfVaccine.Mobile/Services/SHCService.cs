﻿using System;
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
using ProofOfVaccine.Token.Exceptions;
using ProofOfVaccine.Rules.NS.Validator;
using ProofOfVaccine.Rules.NS.Models;
using Newtonsoft.Json;
using System.Reflection;
using System.IO;

namespace ProofOfVaccine.Mobile.Services
{
    public interface ISHCService
    {
        Task InitializeAsync();
        Task<ProofOfVaccinationData> ValidateQRCode(string QRCode);
        Task<ProofOfVaccinationData> ValidateVaccination(string SHCCode);
        ProofOfVaccinationData LastScanData { get; }
        ProofOfVaccinationData InvalidScan();
    }

    public class SHCService : ISHCService
    {
        private const int B64_OFFSET = 45;

        protected readonly IErrorManagementService _errorManagementService;
        protected readonly IDecoder _decoder;
        private readonly IPersistentJwksProvider<IJwksDataStore> _persistentJwksProvider;
        private readonly ILocalDataService _localDataService;

        private Dictionary<Uri, JsonWebKeySet> _whiteListedJwks;
        private IList<ValidVaccine> _validVaccines;
        private JwksCache _defaultCache;

        public SHCService()
        {
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

        public ProofOfVaccinationData InvalidScan()
        {
            return LastScanData = new ProofOfVaccinationData()
            {
                IsValidProof = false,
                Code = VaccineStatus.InvalidFormat,
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

            Stream stream = assembly.GetManifestResourceStream("ProofOfVaccine.Mobile.AppResources.WhiteList.json");

            using (var reader = new StreamReader(stream))
            {
                _whiteListedJwks = LoadWhiteListedJwks(reader.ReadToEnd());
            }

            _defaultCache = new JwksCache(TimeSpan.MaxValue);

            foreach (var set in _whiteListedJwks)
            {
                _defaultCache.Set(set.Key, set.Value);
            }

            stream = assembly.GetManifestResourceStream("ProofOfVaccine.Mobile.AppResources.ValidVaccines.json");

            using (var reader = new StreamReader(stream))
            {
                _validVaccines = LoadValidVaccines(reader.ReadToEnd());
            }
        }

        private IList<ValidVaccine> LoadValidVaccines(string validVaccinesJson)
        {
            return JsonConvert.DeserializeObject<List<ValidVaccine>>(validVaccinesJson);
        }

        private Dictionary<Uri, JsonWebKeySet> LoadWhiteListedJwks(string whiteListJson)
        {
            return JsonConvert.DeserializeObject<Dictionary<Uri, JsonWebKeySet>>(whiteListJson);
        }
    }
}
