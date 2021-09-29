using Newtonsoft.Json;

namespace ProofOfVaccine.Rules.NS.Models
{
    public class ValidVaccine
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; private set; }
        [JsonProperty("code", Required = Required.Always)]
        public string Code { get; private set; }
        [JsonProperty("dosage_count", Required = Required.Always)]
        public int DosageCountRequirement { get; private set; }

        public ValidVaccine(string name,
            string code,
            int dosageCountRequirement)
        {
            Name = name;
            Code = code;
            DosageCountRequirement = dosageCountRequirement;
        }
    }
}
