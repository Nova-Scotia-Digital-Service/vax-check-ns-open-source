using System;
using System.Collections.Generic;
using System.Text;

namespace VaxCheckNS.Mobile.DTOs
{
    public class NetworkInfo : EventArgs
    {
        public bool IsNetworkAvailable { get; }
        public DateTime? LastOnline { get; }
        public DateTime? LastJWKSUpdate { get; }
        public bool IsJWKSOutdated { get; }

        public NetworkInfo(bool isNetworkAvailable, DateTime? lastOnline, DateTime? lastJWKSUpdate, bool isJWKSOutdated = false)
        {
            IsNetworkAvailable = isNetworkAvailable;
            LastOnline = lastOnline;
            LastJWKSUpdate = lastJWKSUpdate;
            IsJWKSOutdated = isJWKSOutdated;
        }
    }
}
