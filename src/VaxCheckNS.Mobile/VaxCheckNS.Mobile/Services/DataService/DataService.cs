using Newtonsoft.Json;
using VaxCheckNS.Mobile.DTOs;
using VaxCheckNS.Token.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace VaxCheckNS.Mobile.Services
{
	public class DataService : IDataService
	{
		public delegate void DataStoredHandler(object sender);
		public event DataStoredHandler DataStored;


		private DateTime? _lastOnlineDate = null;
		public DateTime? LastOnlineDate
		{
			get
			{
				if (_lastOnlineDate == null)
				{
					if (Preferences.ContainsKey(nameof(_lastOnlineDate)))
					{
						var lastOnlineString = Preferences.Get(nameof(_lastOnlineDate), "");
						if (!string.IsNullOrWhiteSpace(lastOnlineString))
						{
							_lastOnlineDate = JsonConvert.DeserializeObject<DateTime?>(lastOnlineString);
						}
					}
				}

				return _lastOnlineDate;
			}
			set
			{
				if (value == null) return;

				_lastOnlineDate = value;

				if (Preferences.ContainsKey(nameof(_lastOnlineDate)))
				{
					Preferences.Remove(nameof(_lastOnlineDate));
				}

				var dateString = JsonConvert.SerializeObject(value);
				Preferences.Set(nameof(_lastOnlineDate), dateString);
			}
		}

		public TimeSpan? SinceLastOnline()
		{
			if (_lastOnlineDate == null) return null;

			return DateTime.UtcNow - _lastOnlineDate.Value;
		}

		private DateTime? _lastJWKSUpdateDate = null;
		public DateTime? LastJWKSUpdateDate
		{
			get
			{
				if (_lastJWKSUpdateDate == null)
				{
					if (Preferences.ContainsKey(nameof(_lastJWKSUpdateDate)))
					{
						var resultString = Preferences.Get(nameof(_lastJWKSUpdateDate), "");
						if (!string.IsNullOrWhiteSpace(resultString))
						{
							_lastJWKSUpdateDate = JsonConvert.DeserializeObject<DateTime?>(resultString);
						}
					}
				}

				return _lastJWKSUpdateDate;
			}
			set
			{
				if (value == null) return;

				_lastJWKSUpdateDate = value;

				if (Preferences.ContainsKey(nameof(_lastJWKSUpdateDate)))
				{
					Preferences.Remove(nameof(_lastJWKSUpdateDate));
				}

				var dateString = JsonConvert.SerializeObject(value);
				Preferences.Set(nameof(_lastJWKSUpdateDate), dateString);
			}
		}

		private Assembly _assembly;
		public DataService(Assembly assembly = null)
		{
			if (assembly == null)
				_assembly = IntrospectionExtensions.GetTypeInfo(typeof(DataService)).Assembly;
			else
				_assembly = assembly;
		}

		private Dictionary<Uri, JWKeySet> _jsonWebKeySets;
		public async Task<Dictionary<Uri, JWKeySet>> LoadJWKSFromOnlineAsync(Dictionary<Uri, JWKeySet> uriKeys)
		{//TODO

			LastJWKSUpdateDate = DateTime.UtcNow;

			return null;
		}
		public async Task<Dictionary<Uri, JWKeySet>> GetJWKSLocallyAsync()
		{
			if (_jsonWebKeySets == null)
				_jsonWebKeySets = await LoadJWKSFromDiskAsync();

			return _jsonWebKeySets;
		}
		public async Task<Dictionary<Uri, JWKeySet>> LoadJWKSFromDiskAsync()
		{//TODO
			try
			{
				var data = await SecureStorage.GetAsync(nameof(_jsonWebKeySets));
				return JsonConvert.DeserializeObject<Dictionary<Uri, JWKeySet>>(data);
			}
			catch (Exception ex)
			{

			}

			return null;
		}

		public async Task<Dictionary<Uri, JWKeySet>> SaveJWKSAsync(Dictionary<Uri, JWKeySet> keySets)
		{//TODO
			try
			{
				await SecureStorage.SetAsync(nameof(_jsonWebKeySets), JsonConvert.SerializeObject(keySets));
				return keySets;
			}
			catch (Exception ex)
			{

			}

			return null;
		}

		private List<Vaccine> _validVaccines;
		public List<Vaccine> GetValidVaccines(bool forceLoadfromfile = false)
		{
			if (_validVaccines == null)
				_validVaccines = new List<Vaccine>();

			if (_validVaccines.Count <= 0 || forceLoadfromfile)
			{
				Stream stream = _assembly.GetManifestResourceStream("VaxCheckNS.Mobile.AppResources.ValidVaccines.json");

				using (var reader = new StreamReader(stream))
				{
					_validVaccines = JsonConvert.DeserializeObject<List<Vaccine>>(reader.ReadToEnd());
				}
			}

			return _validVaccines;
		}

		private Dictionary<Uri, JWKeySet> _whitelistedIssuerKeySets;

		public Dictionary<Uri, JWKeySet> GetWhitelistedIssuerKeySets(bool forceLoadfromfile = false)
		{
			if (_whitelistedIssuerKeySets == null)
				_whitelistedIssuerKeySets = new Dictionary<Uri, JWKeySet>();

			if (_whitelistedIssuerKeySets.Count <= 0 || forceLoadfromfile)
			{
				Stream stream = _assembly.GetManifestResourceStream("VaxCheckNS.Mobile.AppResources.WhiteList.json");

				using (var reader = new StreamReader(stream))
				{
					_whitelistedIssuerKeySets = JsonConvert.DeserializeObject<Dictionary<Uri, JWKeySet>>(reader.ReadToEnd());
				}

			}

			return _whitelistedIssuerKeySets;
		}

		private readonly string privacy_accepted = nameof(privacy_accepted);
        public void AcceptPrivacy()
        {
			Preferences.Set(privacy_accepted, true);
		}

        public bool VerifyPrivacyAccepted()
        {
			return Preferences.Get(privacy_accepted, false);
        }

		private readonly string tou_accepted = nameof(tou_accepted);
		public void AcceptTermsOfUse()
        {
			Preferences.Set(tou_accepted, true);
		}

        public bool VerifyTermsOfUseAccepted()
        {
			return Preferences.Get(tou_accepted, false);
		}

		private readonly string tutorial_complete = nameof(tutorial_complete);
		public void CompleteTutorial()
        {
			Preferences.Set(tutorial_complete, true);
		}

        public bool VerifyTutorialComplete()
        {
			return Preferences.Get(tutorial_complete, false);
		}
    }
}
