using System;
using System.Collections.Generic;
using System.Text;
using ProofOfVaccine.Rules.NS.Models;

namespace ProofOfVaccine.Mobile.DTOs
{
    public class ProofOfVaccinationData
    {
        public string Name => string.Join(" ", GivenName, FamilyName);
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string DateOfBirth { get; set; }

        public bool IsValidProof { get; set; }
        public string SHCCode { get; set; }

        public VaccineStatus Code { get; set; }
    }
}
