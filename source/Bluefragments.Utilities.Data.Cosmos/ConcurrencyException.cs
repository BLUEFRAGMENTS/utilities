using System;
using System.Collections.Generic;
using System.Text;

namespace Bluefragments.Utilities.Data.Cosmos
{
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException(string message)
            : base(message)
        {
        }

        public ConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
