﻿using ProofOfVaccine.Mobile.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProofOfVaccine.Mobile.Services
{
    public class MockDataService : IDataService
    {
        public Task<Dictionary<Uri, JWKeySet>> GetJWKSLocallyAsync()
        {
            throw new NotImplementedException();
        }

        public List<Vaccine> GetValidVaccines(bool forceLoadfromfile = false)
        {
            throw new NotImplementedException();
        }

        public Dictionary<Uri, JWKeySet> GetWhitelistedIssuerKeySets(bool forceLoadfromfile = false)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<Uri, JWKeySet>> LoadJWKSFromDiskAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<Uri, JWKeySet>> LoadJWKSFromOnlineAsync(Dictionary<Uri, JWKeySet> uriKeys)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<Uri, JWKeySet>> SaveJWKSAsync(Dictionary<Uri, JWKeySet> keySets)
        {
            throw new NotImplementedException();
        }
    }
}
