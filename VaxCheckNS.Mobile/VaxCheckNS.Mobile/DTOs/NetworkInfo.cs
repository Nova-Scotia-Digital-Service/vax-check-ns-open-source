using System;
using System.Collections.Generic;
using System.Text;

namespace VaxCheckNS.Mobile.DTOs
{
	public class NetworkInfo : EventArgs
	{
		public bool IsNetworkAvailable { get; }
		public DateTime? LastOnline { get; }
		public TimeSpan? SinceLastOnline { get; }
		public DateTime? LastJWKSUpdate { get; }
		public TimeSpan? SinceLastJWKSUpdate { get; }
		public bool IsJWKSOutdated { get; set; }

		public NetworkInfo(bool isNetworkAvailable,
						   DateTime? lastOnline,
						   DateTime? lastJWKSUpdate,
						   bool isJWKSOutdated = false)
		{
			IsNetworkAvailable = isNetworkAvailable;
			LastOnline = lastOnline.Value.ToLocalTime();
			LastJWKSUpdate = lastJWKSUpdate.Value.ToLocalTime();
			IsJWKSOutdated = isJWKSOutdated;

			if (lastOnline.HasValue)
				SinceLastOnline = DateTime.UtcNow - lastOnline.Value;

			if (lastJWKSUpdate.HasValue)
				SinceLastJWKSUpdate = DateTime.UtcNow - lastJWKSUpdate.Value;
		}

		public NetworkInfo(NetworkInfo data)
		{
			IsNetworkAvailable = data.IsNetworkAvailable;
			LastOnline = data.LastOnline;
			LastJWKSUpdate = data.LastJWKSUpdate;
			IsJWKSOutdated = data.IsJWKSOutdated;

			SinceLastOnline = data.SinceLastOnline;
			SinceLastJWKSUpdate = data.SinceLastJWKSUpdate;
		}
	}
}
