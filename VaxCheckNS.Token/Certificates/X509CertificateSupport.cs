﻿using VaxCheckNS.Token.Encoders;
using VaxCheckNS.Token.Model.Jwks;
using VaxCheckNS.Token.Algorithms;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using VaxCheckNS.Token.Exceptions;

namespace VaxCheckNS.Token.Certificates
{
  public static class X509CertificateSupport
  {
    public static X509Certificate2 GetFirstMatchingCertificate(string FindValue,
          X509FindType FindType, StoreName StoreName,
          StoreLocation StoreLocation, bool Valid)
    {
      X509Store certStore = new X509Store(StoreName, StoreLocation);
      certStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
      X509Certificate2Collection foundCerts = certStore.Certificates.Find(FindType, FindValue, Valid);
      certStore.Close();
      if (foundCerts.Count == 0)
      {
        throw new Exception($"Unable to locate a certificate for the find value of {FindValue} of type {FindType} in the store location of {StoreLocation.ToString()} and store name of {StoreName.ToString()} with a Valid status of {Valid.ToString()}.");
      }
      return foundCerts[0];
    }
  }
}

