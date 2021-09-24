using System;
using System.Collections.Generic;
using System.Text;

namespace ProofOfVaccine.Mobile.DTOs
{
    public class ProofOfVaccinationModel
    {
        public string Name => string.Join(" ", GivenName, FamilyName);
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string DateOfBirth { get; set; }

        public bool IsValidProof { get; set; }
        public string ValidationMessage { get; set; }
    }
}
