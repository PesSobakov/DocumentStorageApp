using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Reflection;
using System.Runtime.Intrinsics.X86;

namespace DocumentStorageApp.Services
{
    public class AzureStorage : IAzureStorage
    {
        private readonly string _storageConnectionString;
        private readonly string _storageContainerName;

        public AzureStorage(IConfiguration configuration)
        {
            _storageConnectionString = configuration.GetValue<string>("BlobConnectionString");
            _storageContainerName = configuration.GetValue<string>("BlobContainerName");
        }

        async public Task UploadAsync(string name, Stream content, string email)
        {
            BlobContainerClient blobContainerClient = new BlobContainerClient(_storageConnectionString, _storageContainerName);
            if (!await blobContainerClient.ExistsAsync())
            {
                await blobContainerClient.CreateAsync();
            }
            await blobContainerClient.DeleteBlobIfExistsAsync(name);

            Dictionary<string, string> metadata = new Dictionary<string, string>
            {
                { "email", email }
            };

            BlobUploadOptions options = new BlobUploadOptions();
            options.Metadata = metadata;

            BlobClient blobClient = new BlobClient(_storageConnectionString, _storageContainerName, name);
            await blobClient.UploadAsync(content, options);
        }
    }
}
