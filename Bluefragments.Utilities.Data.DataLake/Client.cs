using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using Bluefragments.Utilities.Extensions;

namespace Bluefragments.Utilities.Data.DataLake
{
    public class Client : IClient
    {
        public Client(string storageAccountName, string storageAccountKey, string StorageAccountUri)
        {
            SharedKeyCredential = new StorageSharedKeyCredential(storageAccountName, storageAccountKey);
            ServiceClient = new DataLakeServiceClient(new Uri(StorageAccountUri), SharedKeyCredential);
        }

        private StorageSharedKeyCredential SharedKeyCredential { get; }

        public DataLakeServiceClient ServiceClient { get; }

        public async Task WriteBlobAsync(string container, string blobPath, string content, string fileName = null, string folder = null)
        {
            DataLakeFileSystemClient filesystem = ServiceClient.GetFileSystemClient(container);
            var path = Path.Combine(blobPath, folder ?? DateTime.Now.ToEpochTimeSeconds().ToString(), fileName ?? string.Concat(Guid.NewGuid(), ".json"));

            using var stream = content.ToStream();
            var file = filesystem.GetFileClient(path);

            await file.UploadAsync(stream);

            stream.Close();
        }

        public async Task<string> ReadBlobAsync(string storageAccountBlobUri)
        {
            var file = new DataLakeFileClient(new Uri(storageAccountBlobUri), SharedKeyCredential);
            var fileContents = await file.ReadAsync();

            var str = string.Empty;

            using var streamReader = new StreamReader(fileContents.Value.Content);
            str = await streamReader.ReadToEndAsync();

            fileContents.Value.Content.Close();

            return str;
        }

        public async Task DeleteBlobAsync(string storageAccountBlobUri)
        {
            var file = new DataLakeFileClient(new Uri(storageAccountBlobUri), SharedKeyCredential);
            await file.DeleteAsync();
        }

        public List<PathItem> GetFileSystemPathItems(string container, string path)
        {
            var names = new List<PathItem>();

            DataLakeFileSystemClient filesystem = ServiceClient.GetFileSystemClient(container);

            foreach (PathItem pathItem in filesystem.GetPaths(path))
            {
                names.Add(pathItem);
            }

            return names;
        }
    }
}
