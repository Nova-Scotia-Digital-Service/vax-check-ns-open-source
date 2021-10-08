using System;
using System.Collections.Generic;
using System.Text;

namespace VaxCheckNS.Mobile.Helpers
{
    public static class AppSettings
    {
        public static string AppPublicKey = "";
        public static string AppPrivateKey = "";

        public static string AppCenterAndroidKey = "";
        public static string AppCenteriOSKey = "";

        public static TimeSpan JWKSUpdateThreshold => new TimeSpan(0, 0, 1, 0, 0);
    }
}
