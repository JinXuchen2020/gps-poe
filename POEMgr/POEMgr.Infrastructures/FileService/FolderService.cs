
namespace FileService
{
    public class FolderService
    {
        public bool CreateDirectoryIfNotExist(string folder)
        {
            var successed = true;
            try
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
            catch (Exception)
            {
                successed = false;
            }

            return successed;
        }

        public bool CopyFolder(string sourceFolder, string destFolder)
        {
            var successed = true;

            try
            {
                successed = CreateDirectoryIfNotExist(destFolder);

                string[] files = Directory.GetFiles(sourceFolder);
                foreach (string file in files)
                {
                    string name = Path.GetFileName(file);
                    string dest = Path.Combine(destFolder, name);
                    File.Copy(file, dest);
                }

                string[] folders = Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders)
                {
                    string name = Path.GetFileName(folder);
                    string dest = Path.Combine(destFolder, name);
                    CopyFolder(folder, dest);
                }
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
                if (Directory.Exists(folder))
                {
                    successed = ClearFolder(folder);
                    Directory.Delete(folder);
                }
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
                if (Directory.Exists(folder))
                {
                    foreach (var directory in Directory.GetFileSystemEntries(folder))
                        if (File.Exists(directory))
                            File.Delete(directory);
                        else
                            successed = DeleteFolder(directory);
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

            var resultDictionary = new Dictionary<string, string>();
            var directoryInfo = new DirectoryInfo(folder);
            var directories = directoryInfo.GetDirectories();
            var fileInfos = directoryInfo.GetFiles();

            if (includeBaseDirectory)
                namePrefix += directoryInfo.Name + "\\";
            foreach (var directory in directories)
                result.AddRange(GetAllDir(directory.FullName, true, namePrefix));
            foreach (var fileInfo in fileInfos)
                if (!resultDictionary.ContainsKey(fileInfo.FullName))
                    result.Add((fileInfo.FullName, namePrefix + fileInfo.Name, fileInfo.Name));
            return result;
        }
    }
}
