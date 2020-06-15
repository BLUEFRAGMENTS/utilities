using System;
using System.Collections.Generic;

namespace Bluefragments.Utilities.Data.Cosmos
{
    public class BulkOperationResponse<T>
    {
        public TimeSpan TotalTimeTaken { get; set; }

        public int SuccessfullDocuments { get; set; } = 0;

        public double TotalRequestUnitsConsumed { get; set; } = 0;

        public IReadOnlyList<(T, Exception)> Failures { get; set; }
    }
}