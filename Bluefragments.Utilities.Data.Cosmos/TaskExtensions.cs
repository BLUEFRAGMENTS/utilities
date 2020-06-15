using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Bluefragments.Utilities.Data.Cosmos
{
    public static class TaskExtensions
    {
        public static Task<OperationResponse<T>> CaptureOperationResponse<T>(this Task<ItemResponse<T>> task, T item)
        {
            return task.ContinueWith(itemResponse =>
            {
                if (itemResponse.IsCompletedSuccessfully)
                {
                    return new OperationResponse<T>()
                    {
                        Item = item,
                        IsSuccessfull = true,
                        RequestUnitsConsumed = task.Result.RequestCharge
                    };
                }

                AggregateException innerExceptions = itemResponse.Exception.Flatten();
                CosmosException cosmosException = innerExceptions.InnerExceptions.FirstOrDefault(innerEx => innerEx is CosmosException) as CosmosException;
                if (cosmosException != null)
                {
                    return new OperationResponse<T>()
                    {
                        Item = item,
                        RequestUnitsConsumed = cosmosException.RequestCharge,
                        IsSuccessfull = false,
                        CosmosException = cosmosException
                    };
                }

                return new OperationResponse<T>()
                {
                    Item = item,
                    IsSuccessfull = false,
                    CosmosException = innerExceptions.InnerExceptions.FirstOrDefault()
                };
            });
        }
    }
}
