using System;
namespace VaxCheckNS.Rules.NS.Models
{
    public enum VaccineStatus
    {
        ValidVaccine, // default
        InvalidVaccineCode,
        InvalidOccuranceDate,
        InvalidDosageCount,
        InvalidFormat,
        RulesNotInitialized,
        InvalidIssuer
    }
}
