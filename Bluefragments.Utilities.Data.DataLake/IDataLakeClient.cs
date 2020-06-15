using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;

namespace Bluefragments.Utilities.Data.DataLake
{
    public interface IDataLakeClient
    {
        public DataLakeServiceClient ServiceClient { get; }

        Task WriteBlobAsync(string container, string blobPath, string content, string fileName = null, string folder = null);

        Task<string> ReadBlobAsync(string storageAccountBlobUri);

        Task DeleteBlobAsync(string storageAccountBlobUri);

        List<PathItem> GetFileSystemPathItems(string container, string path);
    }
}
    