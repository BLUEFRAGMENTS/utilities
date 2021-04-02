using System;
using System.Collections.Generic;
using System.Linq;

namespace Dpx.Iot.Infrastructure.Extensions
{
    internal static class StringExtensions
    {
        private const char ValuePairDelimiter = ';';
        private const char ValuePairSeparator = '=';
        private const string HostNamePropertyName = "HostName";

        internal static IDictionary<string, string> ToDictionary(this string input, char keyValuePairDelimiter, char keyValuePairSeparator)
        {
            IEnumerable<string[]> keyValuePairs = input
                .Split(keyValuePairDelimiter)
                .Select(part => part.Split(new char[] { keyValuePairSeparator }, 2));

            if (keyValuePairs.Any(keyValuePair => keyValuePair.Length != 2))
            {
                throw new FormatException("Malformed Token");
            }

            return keyValuePairs.ToDictionary((kvp) => kvp[0], (kvp) => kvp[1], StringComparer.OrdinalIgnoreCase);
        }

        internal static string ExtractHostName(this string connectionString)
        {
            IDictionary<string, string> values = connectionString.ToDictionary(ValuePairDelimiter, ValuePairSeparator);
            if (values.TryGetValue(HostNamePropertyName, out var host))
            {
                return host;
            }

            return string.Empty;
        }
    }
}
