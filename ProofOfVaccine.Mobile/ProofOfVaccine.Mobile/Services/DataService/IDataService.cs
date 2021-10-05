﻿using ProofOfVaccine.Mobile.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProofOfVaccine.Mobile.Services
{
    public interface IDataService
    {
        //Task<List<JsonWebKeySet>> GetJWKSAsync();
        Task<Dictionary<Uri, JWKeySet>> LoadJWKSFromOnlineAsync(Dictionary<Uri, JWKeySet> uriKeys);

        Task<Dictionary<Uri, JWKeySet>> GetJWKSLocallyAsync();
        Task<Dictionary<Uri, JWKeySet>> LoadJWKSFromDiskAsync();
        Task<Dictionary<Uri, JWKeySet>> SaveJWKSAsync(Dictionary<Uri, JWKeySet> keySets);

        Dictionary<Uri, JWKeySet> GetWhitelistedIssuerKeySets(bool forceLoadfromfile = false);
        List<Vaccine> GetValidVaccines(bool forceLoadfromfile = false);

        DateTime? LastOnlineDate { get; set; }
        TimeSpan? SinceLastOnline();
        DateTime? LastJWKSUpdateDate { get; }


    }
}