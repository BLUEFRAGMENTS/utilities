using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;

namespace Bluefragments.Utilities.Data.DataLake;

public interface IDataLakeClient
{
    public DataLakeServiceClient ServiceClient { get; }

    Task<T> GetItemAsync<T>(string container, string fileName);

    Task<IEnumerable<T>> GetItemsAsync<T>(string container, string fileName);

    Task<T> UpdateItemAsync<T>(string container, string fileName, T item);

    Task DeleteItemAsync<T>(string container, string fileName);
}
