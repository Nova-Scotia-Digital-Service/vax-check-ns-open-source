using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using VaxCheckNS.Rules.Extensions;
using VaxCheckNS.Rules.NS.Models;
using VaxCheckNS.Rules.Rule;
using VaxCheckNS.Rules.Support;
using VaxCheckNS.Rules.Validator;

namespace VaxCheckNS.Rules.NS.Validator
{
    public class NSVaccineValidator : IRuleValidator<VaccineStatus>
    {
        private const string VaccineResourceTypeKey = "@..resourceType";
        private const string VaccineResourceTypeValue = "'Immunization'";

        private const string VaccineStatusKey = "@..status";
        private const string VaccineStatusValue = "'completed'";

        //TODO: If we ever decide to support other coding system, system/list of codes as dictionary parameter.
        private const string VaccineCodingSystemKey = "@.system";
        private const string VaccineCodingSystemValue = "'http://hl7.org/fhir/sid/cvx'";
        private readonly string VaccineCodeKey
            = $"$..vaccineCode.coding[?({VaccineCodingSystemKey}=={VaccineCodingSystemValue})].code";

        private readonly string EntryKey
            = $"$..entry[?({VaccineResourceTypeKey}=={VaccineResourceTypeValue} && {VaccineStatusKey}=={VaccineStatusValue})]";

        private const int MinimumDaysVaccinated = 14;
        private const int MinimumMultipleDosageCount = 2;
        private const string OccuranceDateTimeKey = "$..occurrenceDateTime";

        private readonly IList<ValidVaccine> _vaccineList;

        private readonly RuleSet<List<JToken>, VaccineStatus> _vaccinationRuleSet;
            
        public NSVaccineValidator(JObject fhirBundle,
            IList<ValidVaccine> vaccineList)
        {
            // TODO: Add check for the fhir versioning so multiple versions can be used at the same time.

            _vaccineList = vaccineList;

            // Find tokens where type is immunization.
            var completedImmunizationResources = fhirBundle.SelectTokens(EntryKey)
                .ToList();

            _vaccinationRuleSet = completedImmunizationResources
               .CreateRuleSet(r => ValidateVaccineCode(r), VaccineStatus.InvalidVaccineCode)
               .AppendRule(r => ValidateVaccineDate(r), VaccineStatus.InvalidOccuranceDate)
               .AppendRule(r => ValidateVaccineCount(r), VaccineStatus.InvalidDosageCount);
        }

        /// <summary>
        /// Validates that any of the codes are valid in NS.
        /// </summary>
        /// <param name="completedImmunizationResources">Each of the immunization entries.</param>
        /// <returns></returns>
        private bool ValidateVaccineCode(List<JToken> completedImmunizationResources)
        {
            return completedImmunizationResources
                .Any(j => _vaccineList
                    .Select(v => v.Code)
                    .Contains(j
                        .SelectToken(VaccineCodeKey)
                        .ToString()));
        }

        /// <summary>
        /// Validate the that both the vaccine dates are greater than or equal to 14 days old.
        /// </summary>
        /// <param name="completedImmunizationResources">Each of the immunization entries.</param>
        /// <returns></returns>
        private bool ValidateVaccineDate(List<JToken> completedImmunizationResources)
        {
            var vaccineCodesFromQR = completedImmunizationResources
                .Select(j => j.SelectToken(VaccineCodeKey).ToString());

            var singleDosageVaccineCodes = _vaccineList
                .Where(v => v.DosageCountRequirement == 1)
                .Select(v => v.Code);

            bool hasValidSingleDose = vaccineCodesFromQR
                .Any(v => singleDosageVaccineCodes.Contains(v));

            if (hasValidSingleDose)
            {
                return completedImmunizationResources
                    .Where(t => singleDosageVaccineCodes
                        .Contains(t.SelectToken(VaccineCodeKey).ToString()))
                    .Select(t => DateTimeOffset
                        .Parse(t.SelectToken(OccuranceDateTimeKey).ToString()))
                    .OrderBy(t => t)
                    .Take(1)
                    .All(t => DateTimeOffset.Now.Subtract(t)
                             >= TimeSpan.FromDays(MinimumDaysVaccinated));
            }
            else
            {
                return completedImmunizationResources
                    .Select(t => DateTimeOffset
                        .Parse(t.SelectToken(OccuranceDateTimeKey).ToString()))
                    .OrderBy(t => t)
                    .Take(2)
                    .All(t => DateTimeOffset.Now.Subtract(t)
                             >= TimeSpan.FromDays(MinimumDaysVaccinated));
            }
        }

        /// <summary>
        /// Validate the number of vaccinations matches the product.
        /// </summary>
        /// <param name="completedImmunizationResources">Each of the immunization entries.</param>
        /// <returns></returns>
        private bool ValidateVaccineCount(List<JToken> completedImmunizationResources) {

            var vaccineCodesFromQR = completedImmunizationResources
                .Select(j => j.SelectToken(VaccineCodeKey).ToString());

            var singleDosageVaccineCodes = _vaccineList
                .Where(v => v.DosageCountRequirement == 1)
                .Select(v => v.Code);

            var multiDosageVaccineCodes = _vaccineList
                .Where(v => v.DosageCountRequirement > 1)
                .Select(v => v.Code);

            bool hasValidSingleDose = vaccineCodesFromQR
                .Any(v => singleDosageVaccineCodes.Contains(v));

            bool hasValidMultiDose = vaccineCodesFromQR
                    .Where(v => multiDosageVaccineCodes.Contains(v))
                    .Count() >= MinimumMultipleDosageCount;

            return hasValidSingleDose || hasValidMultiDose;  
        }

        public Result<VaccineStatus> Validate()
        {
            try
            {
                return _vaccinationRuleSet.Validate(VaccineStatus.RulesNotInitialized);
            }
            catch
            {
                return Result<VaccineStatus>.Fail(VaccineStatus.InvalidFormat);
            }
        }

        public void Dispose()
        {
            _vaccinationRuleSet.Dispose();
        }
    }
}
