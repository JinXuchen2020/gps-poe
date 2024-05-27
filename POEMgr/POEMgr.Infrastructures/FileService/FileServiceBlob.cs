using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using System.IO.Compression;

namespace FileService
{
    public class FileServiceBlob
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;
        private readonly FolderServiceBlob _folderService;
        private readonly ZipService _zipService;

        public FileServiceBlob(AzureBlobProvider provider)
        {
            _blobServiceClient = provider.GetBlobServiceClient();
            _blobContainerClient = provider.GetBlobContainerClient();

            if (_blobContainerClient == null)
            {
                _blobContainerClient = _blobServiceClient.GetBlobContainerClient("default");
                if (!_blobContainerClient.Exists())
                    _blobContainerClient = _blobServiceClient.CreateBlobContainer("default");
            }

            _folderService = new FolderServiceBlob(provider);
            _zipService = new ZipService();
        }

        public bool Save(List<IFormFile> files, string subFolder = null)
        {
            bool successed = true;
            try
            {
                foreach (IFormFile file in files)
                    successed = this.Save(file, string.Empty, subFolder);
            }
            catch (Exception)
            {
                successed = false;
            }
            return successed;
        }

        public bool Save(IFormFile file, string name = null, string subFolder = null)
        {
            bool successed = true;
            try
            {
                string blobName = string.IsNullOrEmpty(subFolder) ? name : subFolder + "/" + name;
                using (FileStream stream = new FileStream(blobName, FileMode.Create, FileAccess.Write))
                {
                    this._blobContainerClient.UploadBlob(name, stream);
                }
            }
            catch (Exception)
            {
                successed = false;
            }
            return successed;
        }

        public string SaveAndReturnSasUri(Stream content, string name, string subFolder = null)
        {
            try
            {
                string blobName = string.IsNullOrEmpty(subFolder) ? name : subFolder + "/" + name;
                content.Position = 0;
                this._blobContainerClient.UploadBlob(blobName, content);
                BlobClient blobClient = _blobContainerClient.GetBlobClient(blobName);
                if (blobClient.CanGenerateSasUri)
                {
                    BlobSasBuilder sasBuilder = new BlobSasBuilder(BlobContainerSasPermissions.Read, DateTimeOffset.UtcNow.AddYears(10))
                    {
                        BlobContainerName = _blobContainerClient.Name,
                        Resource = "b"
                    };
                    Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
                    return sasUri.AbsoluteUri;
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public string SaveAndReturnUri(Stream content, string name, string type, string subFolder = null)
        {
            try
            {
                string blobName = string.IsNullOrEmpty(subFolder) ? name : subFolder + "/" + name;
                content.Position = 0;
                BlobClient blobClient = _blobContainerClient.GetBlobClient(blobName);
                var uploadOptions = new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = type
                    }
                };
                blobClient.DeleteIfExists();
                blobClient.Upload(content, uploadOptions);
                return blobClient.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public bool Delete(string name, string subFolder = null)
        {
            bool successed = true;
            try
            {
                string blobName = string.IsNullOrEmpty(subFolder) ? name : subFolder + "/" + name;
                this._blobContainerClient.DeleteBlob(blobName);
            }
            catch (Exception ex)
            {
                successed = false;
            }
            return successed;
        }

        public async Task<byte[]> GetFile(string name)
        {
            try
            {
                var blob = new BlobClient(new Uri(name));
                using var stream = await blob.OpenReadAsync();
                var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                return ms.ToArray();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public byte[] GetZipFile(string subFolder)
        {
            try
            {
                var azurePath = this.GenerateFolder(subFolder);
                var dirs = this._folderService.GetAllDir(azurePath);
                var tempFolder = $"{Directory.GetCurrentDirectory()}\\TempBlobDownload\\";
                var localFolder = $"{tempFolder}{Guid.NewGuid()}\\";
                if (!Directory.Exists(localFolder)) Directory.CreateDirectory(localFolder);
                foreach (var dir in dirs)
                    this._blobContainerClient.GetBlobClient(dir.RelatedPath).DownloadTo(localFolder + dir.fileName);

                string zipFile = $"{tempFolder}{azurePath.Replace("/", "_")}.zip";
                this._zipService.CreatZip(localFolder, zipFile, CompressionLevel.NoCompression, false);
                byte[] result = File.ReadAllBytes(zipFile);

                File.Delete(zipFile);
                Directory.Delete(localFolder, true);

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<string> ListFiles(string subFolder)
        {
            List<string> files = new List<string>();

            var dirs = this._folderService.GetAllDir(this.GenerateFolder(subFolder));
            foreach (var dir in dirs)
                files.Add(dir.AbsolutePath);

            return files;
        }

        private string GenerateFilePath(string name, string subFolder)
        {
            return this.GenerateFolder(subFolder) + name;
        }

        private string GenerateFolder(string subFolder)
        {
            if (string.IsNullOrEmpty(subFolder))
                return _blobContainerClient.Name + "/";
            else
                return _blobContainerClient.Name + "/" + subFolder.Replace(@"\", "/") + @"/";
        }

    }
}
