using Azure.Storage.Blobs;
using System.IO;

namespace GadgetStoreASPExam.Blob
{
    public class BlobStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public BlobStorageService(string connectionString, string containerName)
        {
            _connectionString = connectionString;
            _containerName = containerName;
        }

        public async Task<string> UploadImageToBlobStorage(Stream imageStream, string fileName)
        {
            BlobContainerClient containerClient = new BlobContainerClient(_connectionString, _containerName);

            await containerClient.CreateIfNotExistsAsync();

            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(imageStream, true);

            return blobClient.Uri.AbsoluteUri;
        }
    }
}
