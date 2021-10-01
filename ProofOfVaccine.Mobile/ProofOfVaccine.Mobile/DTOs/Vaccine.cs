using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProofOfVaccine.Mobile.DTOs
{
    public class Vaccine
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; private set; }
        [JsonProperty("code", Required = Required.Always)]
        public string Code { get; private set; }
        [JsonProperty("dosage_count", Required = Required.Always)]
        public int DosageCountMinRequirement { get; private set; }

        public Vaccine(string name,
                       string code,
                       int dosageCountRequirement)
        {
            Name = name;
            Code = code;
            DosageCountMinRequirement = dosageCountRequirement;
        }
    }
}
