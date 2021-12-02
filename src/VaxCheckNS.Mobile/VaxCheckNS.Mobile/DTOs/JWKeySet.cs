using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace VaxCheckNS.Mobile.DTOs
{
    public class JWKeySet
    {
        [JsonConstructor]
        public JWKeySet(List<JWKey> Keys)
        {
            this.Keys = Keys;
        }

        public JWKeySet()
        {
            this.Keys = new List<JWKey>();
        }

        [JsonProperty("keys", Required = Required.Always)]
        public List<JWKey> Keys { get; set; }
    }
}
