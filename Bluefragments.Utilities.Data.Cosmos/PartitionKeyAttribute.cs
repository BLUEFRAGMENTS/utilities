using System;

namespace Bluefragments.Utilities.Data.Cosmos
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class PartitionKeyAttribute : Attribute
    {
        public PartitionKeyAttribute(bool isPk)
        {
            IsPartitionKey = isPk;
        }

        public bool IsPartitionKey { get; set; }
    }
}
