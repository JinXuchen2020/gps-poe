using FileService.Models;
using ICSharpCode.SharpZipLib.Zip;
using System.IO.Compression;
using System.Text.RegularExpressions;
using ZipFile = System.IO.Compression.ZipFile;

namespace FileService
{
    public class ZipService
    {
        private FolderService _folderService;

        public delegate void UnZipProgressEventHandler(object sender, UnZipProgressEventArgs e);
        public event UnZipProgressEventHandler unZipProgress;

        public delegate void CompressProgressEventHandler(object sender, CompressProgressEventArgs e);
        public event CompressProgressEventHandler compressProgress;

        public ZipService()
        {
            _folderService = new FolderService();
        }

        public bool CreatZip(string sourceDirectoryName, string destinationArchiveFileName, CompressionLevel compressionLevel = CompressionLevel.NoCompression, bool includeBaseDirectory = true)
        {
            int i = 1;
            try
            {
                if (Directory.Exists(sourceDirectoryName))
                    if (!File.Exists(destinationArchiveFileName))
                    {
                        ZipFile.CreateFromDirectory(sourceDirectoryName, destinationArchiveFileName, compressionLevel, includeBaseDirectory);
                    }
                    else
                    {
                        var toZipFileDictionaryList = _folderService.GetAllDir(sourceDirectoryName, includeBaseDirectory);
                        using (var archive = ZipFile.Open(destinationArchiveFileName, ZipArchiveMode.Update))
                        {
                            var count = toZipFileDictionaryList.Count;
                            foreach (var toZipFile in toZipFileDictionaryList)
                            {
                                if (toZipFile.AbsolutePath != destinationArchiveFileName)
                                {
                                    var toZipedFileName = Path.GetFileName(toZipFile.AbsolutePath);
                                    var toDelArchives = new List<ZipArchiveEntry>();
                                    foreach (var zipArchiveEntry in archive.Entries)
                                    {
                                        if (toZipedFileName != null && (zipArchiveEntry.FullName.StartsWith(toZipedFileName) || toZipedFileName.StartsWith(zipArchiveEntry.FullName)))
                                        {
                                            i++;
                                            compressProgress(this, new CompressProgressEventArgs { Size = zipArchiveEntry.Length, Count = count, Index = i, Path = zipArchiveEntry.FullName, Name = zipArchiveEntry.Name });
                                            toDelArchives.Add(zipArchiveEntry);
                                        }
                                    }

                                    foreach (var zipArchiveEntry in toDelArchives)
                                        zipArchiveEntry.Delete();
                                    archive.CreateEntryFromFile(toZipFile.AbsolutePath, toZipFile.RelatedPath, compressionLevel);
                                }
                            }
                        }
                    }
                else if (File.Exists(sourceDirectoryName))
                    if (!File.Exists(destinationArchiveFileName))
                        ZipFile.CreateFromDirectory(sourceDirectoryName, destinationArchiveFileName, compressionLevel, false);
                    else
                    {
                        using (var archive = ZipFile.Open(destinationArchiveFileName, ZipArchiveMode.Update))
                        {
                            if (sourceDirectoryName != destinationArchiveFileName)
                            {
                                var toZipedFileName = Path.GetFileName(sourceDirectoryName);
                                var toDelArchives = new List<ZipArchiveEntry>();
                                var count = archive.Entries.Count;
                                foreach (var zipArchiveEntry in archive.Entries)
                                {
                                    if (toZipedFileName != null && (zipArchiveEntry.FullName.StartsWith(toZipedFileName) || toZipedFileName.StartsWith(zipArchiveEntry.FullName)))
                                    {
                                        i++;
                                        compressProgress(this, new CompressProgressEventArgs { Size = zipArchiveEntry.Length, Count = count, Index = i, Path = zipArchiveEntry.FullName, Name = zipArchiveEntry.Name });
                                        toDelArchives.Add(zipArchiveEntry);
                                    }
                                }

                                foreach (var zipArchiveEntry in toDelArchives)
                                    zipArchiveEntry.Delete();
                                archive.CreateEntryFromFile(sourceDirectoryName, toZipedFileName, compressionLevel);
                            }
                        }
                    }
                else
                    return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CreatZip(Dictionary<string, string> sourceDirectoryName, string destinationArchiveFileName, CompressionLevel compressionLevel = CompressionLevel.NoCompression)
        {
            int i = 1;
            try
            {
                using (FileStream zipToOpen = new FileStream(destinationArchiveFileName, FileMode.OpenOrCreate))
                {
                    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                    {
                        foreach (var toZipFileKey in sourceDirectoryName.Keys)
                        {
                            if (toZipFileKey != destinationArchiveFileName)
                            {
                                var toZipedFileName = Path.GetFileName(toZipFileKey);
                                var toDelArchives = new List<ZipArchiveEntry>();
                                var count = archive.Entries.Count;
                                foreach (var zipArchiveEntry in archive.Entries)
                                {
                                    if (toZipedFileName != null && (zipArchiveEntry.FullName.StartsWith(toZipedFileName) || toZipedFileName.StartsWith(zipArchiveEntry.FullName)))
                                    {
                                        i++;
                                        compressProgress(this, new CompressProgressEventArgs { Size = zipArchiveEntry.Length, Count = count, Index = i, Path = toZipedFileName });
                                        toDelArchives.Add(zipArchiveEntry);
                                    }
                                }

                                foreach (var zipArchiveEntry in toDelArchives)
                                    zipArchiveEntry.Delete();
                                archive.CreateEntryFromFile(toZipFileKey, sourceDirectoryName[toZipFileKey], compressionLevel);
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UnZip(string zipFilePath, string unZipDir)
        {
            bool successed = true;
            try
            {
                unZipDir = unZipDir.EndsWith(@"\") ? unZipDir : unZipDir + @"\";
                var directoryInfo = new DirectoryInfo(unZipDir);
                if (!directoryInfo.Exists)
                    directoryInfo.Create();
                var fileInfo = new FileInfo(zipFilePath);
                if (!fileInfo.Exists)
                    return false;
                using (var zipToOpen = new FileStream(zipFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                {
                    using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                    {
                        var count = archive.Entries.Count;
                        for (int i = 0; i < count; i++)
                        {
                            var entries = archive.Entries[i];
                            if (!entries.FullName.EndsWith("/"))
                            {
                                var entryFilePath = Regex.Replace(entries.FullName.Replace("/", @"\"), @"^\\*", "");
                                var filePath = directoryInfo + entryFilePath;
                                unZipProgress(this, new UnZipProgressEventArgs { Size = entries.Length, Count = count, Index = i + 1, Path = entries.FullName, Name = entries.Name });
                                var content = new byte[entries.Length];
                                entries.Open().Read(content, 0, content.Length);
                                var greatFolder = Directory.GetParent(filePath);
                                if (!greatFolder.Exists)
                                    greatFolder.Create();
                                File.WriteAllBytes(filePath, content);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                successed = false;
            }
            return successed;
        }

        public List<string> GetZipFileList(string zipFilePath)
        {
            List<string> fList = new List<string>();
            if (!File.Exists(zipFilePath))
                return fList;
            try
            {
                using (var zipToOpen = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                    {
                        foreach (var zipArchiveEntry in archive.Entries)
                            if (!zipArchiveEntry.FullName.EndsWith("/"))
                                fList.Add(Regex.Replace(zipArchiveEntry.FullName.Replace("/", @"\"), @"^\\*", ""));
                    }
                }
            }
            catch (Exception)
            {

            }
            return fList;
        }

        public async Task<byte[]> CompressionFilesReturnByte(List<string> sourceFilePaths)
        {
            MemoryStream ms = new MemoryStream();
            ZipOutputStream zos = new ZipOutputStream(ms);
            try
            {
                zos.SetLevel(6);
                foreach (string file in sourceFilePaths)
                {
                    using (FileStream fs = File.OpenRead(file))
                    {
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                        entry.DateTime = DateTime.Now;
                        entry.Size = fs.Length;
                        fs.Close();
                        zos.PutNextEntry(entry);
                        await zos.WriteAsync(buffer, 0, buffer.Length);
                    }
                }
                zos.Finish();
                return ms.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                zos.Close();
                ms.Close();
            }
        }

        public async Task<byte[]> CompressionFilesReturnByte(List<CompressionFilesReturnByteArgs> p)
        {
            using MemoryStream ms = new MemoryStream();
            using ZipOutputStream zos = new ZipOutputStream(ms);
            try
            {
                zos.SetLevel(6);
                foreach (CompressionFilesReturnByteArgs item in p)
                {
                    ZipEntry entry = new ZipEntry(item.Name);
                    entry.DateTime = DateTime.Now;
                    entry.Size = item.Stream.Length;
                    zos.PutNextEntry(entry);
                    await zos.WriteAsync(item.Stream, 0, item.Stream.Length);
                }
                zos.Finish();
                ms.Position = 0;
                return ms.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                zos.Close();
                ms.Close();
            }
        }

        public async Task<byte[]> DownFilesReturnByte(string path)
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    byte[] infbytes = new byte[(int)fs.Length];
                    await fs.ReadAsync(infbytes, 0, infbytes.Length);
                    fs.Close();
                    return infbytes;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ms.Close();
            }
        }
    }
}

