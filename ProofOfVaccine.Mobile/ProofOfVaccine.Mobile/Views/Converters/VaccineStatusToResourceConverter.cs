using System;
using System.Globalization;
using ProofOfVaccine.Mobile.Helpers;
using ProofOfVaccine.Rules.NS.Models;
using Xamarin.Forms;

namespace ProofOfVaccine.Mobile.Converters
{
    public class VaccineStatusToResourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is VaccineStatus vaccineStatus)
            {
                switch (vaccineStatus)
                {
                    case VaccineStatus.InvalidVaccineCode:
                        return new TranslationHelper()
                            .ProvideValue(nameof(AppResources.TextResources.VaccineCodeInvalidText));
                    case VaccineStatus.InvalidOccuranceDate:
                        return new TranslationHelper()
                            .ProvideValue(nameof(AppResources.TextResources.VaccineDateInvalidText));
                    case VaccineStatus.InvalidDosageCount:
                        return new TranslationHelper()
                            .ProvideValue(nameof(AppResources.TextResources.VaccineDosageInvalidText));
                    case VaccineStatus.InvalidFormat:
                        return new TranslationHelper()
                            .ProvideValue(nameof(AppResources.TextResources.FhirFormatInvalidText));
                    case VaccineStatus.InvalidIssuer:
                        return new TranslationHelper()
                            .ProvideValue(nameof(AppResources.TextResources.VaccineInvalidSHCCodeText));
                    case VaccineStatus.ValidVaccine:
                    case VaccineStatus.RulesNotInitialized:
                    default:
                        return string.Empty;
                }

            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
