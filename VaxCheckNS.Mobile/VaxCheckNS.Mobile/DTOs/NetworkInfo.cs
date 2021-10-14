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
			IsJWKSOutdated = isJWKSOutdated;

			if (lastOnline.HasValue)
			{
				LastOnline = lastOnline.Value.ToLocalTime();
				SinceLastOnline = DateTime.UtcNow - lastOnline.Value;
			}

			if (lastJWKSUpdate.HasValue)
			{
				LastJWKSUpdate = lastJWKSUpdate.Value.ToLocalTime();
				SinceLastJWKSUpdate = DateTime.UtcNow - lastJWKSUpdate.Value;
			}
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
