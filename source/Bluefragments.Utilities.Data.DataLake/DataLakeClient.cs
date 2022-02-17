using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using Bluefragments.Utilities.Extensions;
using Newtonsoft.Json;

namespace Bluefragments.Utilities.Data.DataLake;

public class DataLakeClient : IDataLakeClient
{
    public DataLakeClient(string storageAccountName, string storageAccountKey, string storageAccountUri)
    {
        SharedKeyCredential = new StorageSharedKeyCredential(storageAccountName, storageAccountKey);
        ServiceClient = new DataLakeServiceClient(new Uri(storageAccountUri), SharedKeyCredential);
    }

    public DataLakeClient(string connectionString)
    {
        ServiceClient = new DataLakeServiceClient(connectionString);
    }

    public DataLakeServiceClient ServiceClient { get; }

    private StorageSharedKeyCredential SharedKeyCredential { get; }

    public async Task<T> GetItemAsync<T>(string container!!, string fileName!!)
    {
        var fileClient = await GetFileClientAsync(container, fileName, false);
        ArgumentNullException.ThrowIfNull(fileClient);

        if (await fileClient.ExistsAsync() == false)
        {
            return default(T);
        }

        var fileStream = await fileClient.OpenReadAsync();
        using var reader = new StreamReader(fileStream);

        var fileContent = await reader.ReadToEndAsync();
        if (fileContent != null)
        {
            var blobItem = JsonConvert.DeserializeObject<T>(fileContent);
            if (blobItem != null)
            {
                return blobItem;
            }
        }

        return default(T);
    }

    public async Task<IEnumerable<T>> GetItemsAsync<T>(string container!!, string fileName!!)
    {
        var fileClient = await GetFileClientAsync(container, fileName, false);
        ArgumentNullException.ThrowIfNull(fileClient);

        if (await fileClient.ExistsAsync() == false)
        {
            return null;
        }

        var fileStream = await fileClient.OpenReadAsync();
        using var reader = new StreamReader(fileStream);

        var fileContent = await reader.ReadToEndAsync();
        if (fileContent != null)
        {
            var blobItem = JsonConvert.DeserializeObject<IEnumerable<T>>(fileContent);
            if (blobItem != null)
            {
                return blobItem;
            }
        }

        return null;
    }

    public async Task<T> UpdateItemAsync<T>(string container!!, string fileName!!, T item)
    {
        var stream = GetStreamFromBlobFile(fileName, item);
        var fileClient = await GetFileClientAsync(container, fileName, true, true);
        await fileClient.UploadAsync(stream);

        return item;
    }

    public async Task DeleteItemAsync<T>(string container!!, string fileName!!)
    {
        var fileSystemClient = GetFileSystemClient(container);
        var file = fileSystemClient.GetFileClient(fileName);

        await file.DeleteIfExistsAsync();
    }

    private DataLakeFileSystemClient GetFileSystemClient(string container!!)
    {
        return ServiceClient.GetFileSystemClient(container.ToString());
    }

    private async Task<DataLakeFileClient> GetFileClientAsync(string container!!, string fileName!!, bool ensureDirectoryExists = true, bool cleanFile = false)
    {
        var fileSystemClient = GetFileSystemClient(container);
        var file = fileSystemClient.GetFileClient(fileName);

        if (ensureDirectoryExists)
        {
            EnsureDirectoryExists(fileName);
        }

        if (cleanFile)
        {
            await file.DeleteIfExistsAsync();
        }

        return file;
    }

    private static Stream GetStreamFromBlobFile(string itemPath, object item)
    {
        ArgumentNullException.ThrowIfNull(itemPath);
        ArgumentNullException.ThrowIfNull(item);

        var stream = new MemoryStream();
        var streamWriter = new StreamWriter(stream);
        var jsonWriter = new JsonTextWriter(streamWriter);

        var serializer = new JsonSerializer();
        serializer.Serialize(jsonWriter, item);
        jsonWriter.Flush();
        streamWriter.Flush();
        stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }

    private static bool EnsureDirectoryExists(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        var directory = Path.GetDirectoryName(filePath);
        if (directory != null)
        {
            if (Directory.Exists(directory))
            {
                return true;
            }

            Directory.CreateDirectory(directory);
            return true;
        }

        return false;
    }
}
