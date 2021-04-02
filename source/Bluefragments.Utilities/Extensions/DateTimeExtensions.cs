using System;

namespace Bluefragments.Utilities.Extensions
{
    public static class DateTimeExtensions
    {
        public static long ToEpochTimeSeconds(this DateTime dateTime)
        {
            var epochDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dateTime - epochDate).TotalSeconds;
        }
    }
}
