using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProofOfVaccine.Token.Enums;
using ProofOfVaccine.Token.Exceptions;

namespace ProofOfVaccine.Token.Model.Shc
{
  public class VerifiableCredential
  {
    public VerifiableCredential(List<VerifiableCredentialType> VerifiableCredentialTypeList, CredentialSubject CredentialSubject)
    {
      this.VerifiableCredentialTypeList = VerifiableCredentialTypeList;
      this.CredentialSubject = CredentialSubject;
    }

    [Newtonsoft.Json.JsonProperty(propertyName: "type", ItemConverterType = typeof(VerifiableCredentialTypeConverter), Required = Required.Always)]
    public List<VerifiableCredentialType> VerifiableCredentialTypeList { get; set; }
    [JsonProperty("credentialSubject", Required = Required.Always)]
    public CredentialSubject CredentialSubject { get; set; }

  }
}
