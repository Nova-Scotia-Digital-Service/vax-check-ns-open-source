using System;
using System.Collections.Generic;
using System.Text;

namespace VaxCheckNS.Mobile.Helpers
{
    public static class AppSettings
    {
        public static string AppPublicKey = "";
        public static string AppPrivateKey = "A7848922-C10A-4D6A-9D82-4987F638F718";

        public static string AppCenterAndroidKey = "c8da9da7-50dd-42a8-b7f4-312370215d5a";
        public static string AppCenteriOSKey = "b680b5fc-214c-49a1-9bce-5decfe8b445a";

        public static TimeSpan JWKSUpdateThreshold => new TimeSpan(0, 0, 1, 0, 0);
    }
}
