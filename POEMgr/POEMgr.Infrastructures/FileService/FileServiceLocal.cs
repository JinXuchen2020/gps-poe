using Microsoft.AspNetCore.Http;
using System.IO.Compression;

namespace FileService
{
    public class FileServiceLocal
    {
        private readonly string _basePath = @"C:\POEFiles";
        private readonly FolderService _folderService;
        private readonly ZipService _zipService;

        public FileServiceLocal()
        {
            _folderService = new FolderService();
            _zipService = new ZipService();
            _folderService.CreateDirectoryIfNotExist(_basePath);
        }

        public FileServiceLocal(string basePath)
        {
            _basePath = basePath;
            _folderService = new FolderService();
            _zipService = new ZipService();
            _folderService.CreateDirectoryIfNotExist(_basePath);
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
                string targetFileName = this.GenerateFilePath(string.IsNullOrEmpty(name) ? file.FileName : name, subFolder);
                using (FileStream stream = new FileStream(targetFileName, FileMode.Create, FileAccess.Write))
                {
                    file.CopyTo(stream);
                }
            }
            catch (Exception)
            {
                successed = false;
            }
            return successed;
        }

        public bool Save(Stream content, string name, string subFolder = null)
        {
            bool successed = true;
            try
            {
                using (FileStream stream = new FileStream(this.GenerateFilePath(name, subFolder), FileMode.Create, FileAccess.Write))
                {
                    content.CopyTo(stream);
                }
            }
            catch (Exception)
            {
                successed = false;
            }
            return successed;
        }

        public List<string> ListFiles(string subFolder)
        {
            List<string> files = new List<string>();

            var dirs = this._folderService.GetAllDir(this.GenerateFolder(subFolder));
            foreach (var dir in dirs)
                files.Add(dir.AbsolutePath);

            return files;
        }

        public byte[] GetZipFile(string subFolder)
        {
            try
            {
                List<string> subfolders = subFolder.Split(@"\", StringSplitOptions.RemoveEmptyEntries).ToList();
                string zipFile = this.GenerateFilePath(string.Join("_", subfolders) + ".zip", string.Empty);
                this._zipService.CreatZip(this.GenerateFolder(subFolder), zipFile, CompressionLevel.NoCompression, false);
                byte[] result = File.ReadAllBytes(zipFile);

                File.Delete(zipFile);

                return result;

            }
            catch (Exception)
            {
                return null;
            }
        }

        private string GenerateFilePath(string name, string subFolder)
        {
            return this.GenerateFolder(subFolder) + name;
        }

        private string GenerateFolder(string subFolder)
        {
            if (string.IsNullOrEmpty(subFolder))
                return _basePath.EndsWith(@"\") ? _basePath : _basePath + @"\";
            else
                return _basePath.EndsWith(@"\") ? _basePath + subFolder + @"\" : _basePath + @"\" + subFolder + @"\";
        }

    }
}
