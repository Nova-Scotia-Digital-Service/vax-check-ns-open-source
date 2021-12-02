﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace VaxCheckNS.Mobile.DTOs
{
    public class JWKey
    {
        public JWKey(string Kty, string Kid, string Use, string Alg, string Crv, string X, string Y)
        {
            this.Kty = Kty;
            this.Kid = Kid;
            this.Use = Use;
            this.Alg = Alg;
            this.Crv = Crv;
            this.X = X;
            this.Y = Y;
        }

        [JsonProperty("kty", Required = Required.Always)]
        public string Kty { get; set; }
        [JsonProperty("kid", Required = Required.Always)]
        public string Kid { get; set; }
        [JsonProperty("use", Required = Required.Always)]
        public string Use { get; set; }
        [JsonProperty("alg", Required = Required.Always)]
        public string Alg { get; set; }
        [JsonProperty("crv", Required = Required.Always)]
        public string Crv { get; set; }
        [JsonProperty("x", Required = Required.Always)]
        public string X { get; set; }
        [JsonProperty("y", Required = Required.Always)]
        public string Y { get; set; }
    }
}

