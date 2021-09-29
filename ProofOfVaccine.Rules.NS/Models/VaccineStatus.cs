using System;
namespace ProofOfVaccine.Rules.NS.Models
{
    public enum VaccineStatus
    {
        ValidVaccine, // default
        InvalidVaccineCode,
        InvalidOccuranceDate,
        InvalidDosageCount,
        InvalidFormat,
        RulesNotInitialized
    }
}
