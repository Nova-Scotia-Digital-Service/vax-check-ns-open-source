﻿using Newtonsoft.Json;
using VaxCheckNS.Token.Serializers.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaxCheckNS.Token.Model.Shc
{
  public class SmartHealthCardFile
  {   
    public SmartHealthCardFile()
    {     
      VerifiableCredentialList = new List<string>();
    }

    [JsonConstructor]
    public SmartHealthCardFile(List<string> verifiableCredentialList)
    {
      VerifiableCredentialList = verifiableCredentialList;
    }

    [JsonProperty("verifiableCredential", Required = Required.Always)]
    public List<string> VerifiableCredentialList { get; set; }
  }
}
