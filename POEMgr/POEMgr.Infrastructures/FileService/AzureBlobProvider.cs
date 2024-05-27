using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace FileService
{
    public class AzureBlobProvider
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;

        public BlobServiceClient GetBlobServiceClient() => this._blobServiceClient;
        public BlobContainerClient GetBlobContainerClient() => this._blobContainerClient;

        public AzureBlobProvider(string conncetionStr)
        {
            _blobServiceClient = new BlobServiceClient(conncetionStr);
        }

        public AzureBlobProvider(string conncetionStr, string containerName)
        {
            _blobServiceClient = new BlobServiceClient(conncetionStr);
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            if (!_blobContainerClient.Exists())
                _blobContainerClient.CreateIfNotExists(PublicAccessType.BlobContainer);
        }
    }
}
