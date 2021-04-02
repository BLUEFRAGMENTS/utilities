using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Bluefragments.Utilities.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Throws ArgumentOutOfRangeException if parameter is null or whitespace.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="parameterName">Parameter name used for exception message.</param>
        public static void ThrowIfParameterIsNullOrWhiteSpace(this string parameter, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                throw new ArgumentOutOfRangeException(parameterName, "Cannot be null or whitespace.");
            }
        }

        public static IEnumerable<string> SplitInParts(this string s, int partLength)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }

            if (partLength <= 0)
            {
                throw new ArgumentException("Part length has to be positive.", "partLength");
            }

            for (var i = 0; i < s.Length; i += partLength)
            {
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
            }
        }

        public static int FromHexToDecimal(this string hexValue)
        {
            return int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
        }

        public static double FromHexToDouble(this string hexValue)
        {
            return Convert.ToInt64(hexValue, 16);
        }

        public static byte[] StringToByteArray(this string hexValue)
        {
            int numberChars = hexValue.Length;

            if (numberChars % 2 == 0)
            {
                byte[] bytes = new byte[numberChars / 2];
                for (int i = 0; i < numberChars; i += 2)
                {
                    bytes[i / 2] = Convert.ToByte(hexValue.Substring(i, 2), 16);
                }

                return bytes;
            }
            else
            {
                throw new ArgumentException("Hexadecimal string must have even number of character count", "NumberChars");
            }
        }

        public static double FromBinaryToDecimal(this string binaryValue)
        {
            return Convert.ToDouble(Convert.ToString(Convert.ToInt64(binaryValue, 2), 10));
        }

        public static Stream ToStream(this string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static long ToEpochTimeSeconds(this string dateTime, DateTimeStyles dateTimeStyles = DateTimeStyles.RoundtripKind)
        {
            var newDateTime = DateTime.Parse(dateTime, null, dateTimeStyles);

            return newDateTime.ToEpochTimeSeconds();
        }
    }
}
