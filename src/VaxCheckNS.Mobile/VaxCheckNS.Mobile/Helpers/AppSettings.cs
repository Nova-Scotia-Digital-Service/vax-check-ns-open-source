using System;
using System.Collections.Generic;
using System.Text;

namespace VaxCheckNS.Mobile.Helpers
{
    public static class AppSettings
    {
        public static string AppPrivateKey = "A7848922-C10A-4D6A-9D82-4987F638F718"; //Used only internally for thread lock

        public static string AppCenterAndroidKey = "FAKE-ID";
        public static string AppCenteriOSKey = "FAKE-ID";

        public static TimeSpan JWKSUpdateThreshold => new TimeSpan(7, 0, 0, 0, 0); 
    }
}
