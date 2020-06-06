using System;

namespace Bluefragments.Utilities.Extensions
{
	public static class NumericExtensions
	{
		public static DateTime FromEpochTimeToUtcSeconds(this long unixTime)
		{
			var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return epoch.AddSeconds(unixTime);
		}

        public static DateTime FromEpochTimeToUtcMiliseconds(this long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(unixTime);
        }
    }
}
