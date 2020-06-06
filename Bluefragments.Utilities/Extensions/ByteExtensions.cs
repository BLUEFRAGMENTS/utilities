using System;
using System.Collections;

namespace Bluefragments.Utilities.Extensions
{
	public static class ByteExtensions
	{
		// higher means first four bits
		public static int HighBit(this byte byteToHighBits)
		{
			return byteToHighBits >> 4;
		}

		// lower means last four bits
		public static int LowBit(this byte byteToLowBits)
		{
			return byteToLowBits & 0x0F;
		}

		// get a single bit
		public static int GetBit(this byte b, int bitNumber)
		{
			return (b & (1 << bitNumber));
		}

		private static int GetIntFromBitArray(this BitArray bitArray)
		{
			if (bitArray.Length > 32)
				throw new ArgumentException("Argument length shall be at most 32 bits.");

			int[] array = new int[1];
			bitArray.CopyTo(array, 0);
			return array[0];
		}
	}
}
