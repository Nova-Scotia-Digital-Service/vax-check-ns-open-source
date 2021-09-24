using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using ProofOfVaccine.Rules.Extensions;
using ProofOfVaccine.Rules.Rule;
using ProofOfVaccine.Rules.Support;

namespace ProofOfVaccine.Rules.Validator
{
    public class NSVaccineValidator : IRuleValidator
    {
        private const string VaccineResourceTypeKey = "@..resourceType";
        private const string VaccineResourceTypeValue = "'Immunization'";

        private const string VaccineStatusKey = "@..status";
        private const string VaccineStatusValue = "'completed'";

        private const string VaccineCodeKey = "$..vaccineCode..code";

        private readonly string EntryKey
            = $"$..entry[?({VaccineResourceTypeKey}=={VaccineResourceTypeValue} && {VaccineStatusKey}=={VaccineStatusValue})]";

        private const int MinimumDaysVaccinated = 14;
        private const string OccuranceDateTimeKey = "$..occurrenceDateTime";

        private readonly Rule<List<JToken>> _vaccinationRule;
        private readonly IList<string> _validVaccineCodes;
        private readonly IList<string> _singleDosageCodes;

        private readonly string _invalidFormatMessage;
            
        public NSVaccineValidator(JObject fhirBundle,
            IList<string> validVaccineCodes,
            IList<string> singleDosageCodes,
            string invalidVaccineMessage,
            string invalidVaccineDateMessage,
            string invalidVaccineCountMessage,
            string invalidFormatMessage)
        {
            _validVaccineCodes = validVaccineCodes;
            _singleDosageCodes = singleDosageCodes;
            _invalidFormatMessage = invalidFormatMessage;

            // TODO: Add check for the fhir versioning so multiple versions can be used at the same time.

            // Find tokens where type is immunization.
            var completedImmunizationResources = fhirBundle.SelectTokens(EntryKey)
                .ToList();

            _vaccinationRule = completedImmunizationResources
               .CreateRule(r => ValidateVaccineCode(r), invalidVaccineMessage)
               .AppendRule(r => ValidateVaccineDate(r), invalidVaccineDateMessage)
               .AppendRule(r => ValidateVaccineCount(r), invalidVaccineCountMessage);
        }

        /// <summary>
        /// Validate all codes are valid in NS.
        /// </summary>
        /// <param name="completedImmunizationResources">Each of the immunization resources.</param>
        /// <returns></returns>
        private bool ValidateVaccineCode(List<JToken> completedImmunizationResources)
        {
            return completedImmunizationResources
                .All(j => _validVaccineCodes
                    .Contains(j
                        .SelectToken(VaccineCodeKey)
                        .ToString()));
        }

        /// <summary>
        /// Validate the that both the vaccine dates are greater than or equal to 14 days old.
        /// </summary>
        /// <param name="completedImmunizationResources">Each of the immunization resources.</param>
        /// <returns></returns>
        private bool ValidateVaccineDate(List<JToken> completedImmunizationResources)
        {
            return completedImmunizationResources
                .All(t => DateTimeOffset.Now.Subtract(DateTimeOffset
                    .Parse(t.SelectToken(OccuranceDateTimeKey).ToString()))
                         >= TimeSpan.FromDays(MinimumDaysVaccinated));
        }

        /// <summary>
        /// Validate the number of vaccinations matches the product.
        /// </summary>
        ///  /// <param name="completedImmunizationResources">Each of the immunization resources.</param>
        /// <returns></returns>
        private bool ValidateVaccineCount(List<JToken> completedImmunizationResources) {

            bool isSingleDose = _singleDosageCodes
                .Contains(completedImmunizationResources
                    .First()
                    .SelectToken(VaccineCodeKey)
                    .ToString());

            // If is a single dose vaccine, return true, otherwise check if it has at least 2.
            return isSingleDose || completedImmunizationResources.Count >= 2;  
        }

        public Result Validate()
        {
            try
            {
                return _vaccinationRule.Validate();
            }
            catch
            {
                return Result.Fail(_invalidFormatMessage);
            }
        }

        public void Dispose()
        {
            _vaccinationRule.Dispose();
        }
    }
}
