using System;

namespace Bluefragments.Utilities.Data.Cosmos
{
    public class OperationResponse<T>
    {
        public T Item { get; set; }

        public double RequestUnitsConsumed { get; set; } = 0;

        public bool IsSuccessfull { get; set; }

        public Exception CosmosException { get; set; }
    }
}