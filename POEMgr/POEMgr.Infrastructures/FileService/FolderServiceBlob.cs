using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace FileService
{
    public class FolderServiceBlob
    {
        private readonly BlobServiceClient _blobServiceClient;

        public FolderServiceBlob(AzureBlobProvider provider)
        {
            this._blobServiceClient = provider.GetBlobServiceClient();
        }

        public bool CopyFolder(string sourceFolder, string destFolder)
        {
            var successed = true;
            try
            {
                var folderInfoSource = ParseDir(sourceFolder);
                if (string.IsNullOrEmpty(folderInfoSource.containerName)) return false;
                var blobContainerClientSource = _blobServiceClient.GetBlobContainerClient(folderInfoSource.containerName);
                var blobs = this.GetAllDir(sourceFolder);

                var folderInfoDest = ParseDir(sourceFolder);
                if (string.IsNullOrEmpty(folderInfoDest.containerName)) return false;
                var blobContainerClientDest = _blobServiceClient.GetBlobContainerClient(folderInfoDest.containerName);

                foreach (var blob in blobs)
                {
                    var sourceBlobClient = blobContainerClientSource.GetBlobClient(blob.RelatedPath);
                    var sourceBlobSasToken = sourceBlobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.Now.AddMinutes(3));

                    var destBlobClient = blobContainerClientSource.GetBlobClient(folderInfoDest.dir + blob.fileName);
                    destBlobClient.StartCopyFromUri(sourceBlobSasToken);
                }
            }
            catch (Exception)
            {
                successed = false;
            }

            return successed;
        }

        public bool CreateDirectoryIfNotExist(string folder)
        {
            var successed = true;
            try
            {
                var folderInfo = ParseDir(folder);
                if (string.IsNullOrEmpty(folderInfo.containerName)) return false;
                var blobContainerClient = _blobServiceClient.GetBlobContainerClient(folderInfo.containerName);

                //In Azure Storage container, an empty folder can NOT be created, so upload an PlaceHolderBlob.txt to the folder we need to create 
                blobContainerClient.UploadBlob(folderInfo.dir + "PlaceHolderBlob.txt", new BinaryData(string.Empty));
            }
            catch (Exception)
            {
                successed = false;
            }

            return successed;
        }

        public bool ClearFolder(string folder)
        {
            var successed = true;
            try
            {
                this.DeleteFolder(folder);
                this.CreateDirectoryIfNotExist(folder);
            }
            catch (Exception)
            {
                successed = false;
            }

            return successed;
        }

        public bool DeleteFolder(string folder)
        {
            var successed = true;
            try
            {
                var folderInfo = ParseDir(folder);
                if (string.IsNullOrEmpty(folderInfo.containerName)) return false;
                var blobContainerClient = this.CreateContainerIfNotExist(folderInfo.containerName);

                var blobs = this.GetAllDir(folder);
                foreach (var blob in blobs)
                {
                    blobContainerClient.DeleteBlobAsync(blob.RelatedPath);
                }
            }
            catch (Exception)
            {
                successed = false;
            }

            return successed;
        }

        public List<(string AbsolutePath, string RelatedPath, string fileName)> GetAllDir(string folder, bool includeBaseDirectory = false, string namePrefix = "")
        {
            List<(string AbsolutePath, string RelatedPath, string fileName)> result = new List<(string AbsolutePath, string RelatedPath, string fileName)>();

            var folderInfo = ParseDir(folder);
            if (string.IsNullOrEmpty(folderInfo.containerName)) return result;

            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(folderInfo.containerName);

            var resultSegment = blobContainerClient.GetBlobs(BlobTraits.None, BlobStates.None, folderInfo.dir).AsPages();
            foreach (Azure.Page<BlobItem> blobPage in resultSegment)
                foreach (BlobItem blobItem in blobPage.Values)
                    result.Add((blobContainerClient.Uri.AbsoluteUri + "/" + blobItem.Name, blobItem.Name, blobItem.Name.Substring(blobItem.Name.LastIndexOf("/") + 1)));

            return result;
        }

        private BlobContainerClient CreateContainerIfNotExist(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            if (!blobContainerClient.Exists())
                blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            return blobContainerClient;
        }

        //Folder format: <Conatiner Name>/<folder level1>/<folder level2>/
        //Folder example DemoContainer/Folder/
        private (string containerName, string dir) ParseDir(string folder)
        {
            string[] pathSeg = folder.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
            string containerName = pathSeg[0];
            var dir = string.Join('/', pathSeg).Substring(containerName.Length + 1) + "/";

            return (containerName, dir);
        }
    }
}
