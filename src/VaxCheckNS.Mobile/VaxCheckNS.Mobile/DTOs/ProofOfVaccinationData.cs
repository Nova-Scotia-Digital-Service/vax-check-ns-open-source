using System;
using System.Collections.Generic;
using System.Text;
using VaxCheckNS.Rules.NS.Models;

namespace VaxCheckNS.Mobile.DTOs
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

        public string Issuer { get; set; }
    }
}
