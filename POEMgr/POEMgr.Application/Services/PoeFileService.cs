using FileService;
using FileService.Models;
using Microsoft.AspNetCore.Http;
using POEMgr.Application.Interfaces;
using POEMgr.Repository.DBContext;
using POEMgr.Repository.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEMgr.Application.Services
{
    internal class PoeFileService : IPoeFileService
    {
        private readonly FileServiceBlob _fileServiceBlob;
        private readonly ZipService _zipService;
        private readonly DotNetZipService _dotnetZipService;
        private readonly ExcelService _excelService;

        public PoeFileService(AzureBlobProvider provider) 
        {
            _fileServiceBlob = new FileServiceBlob(provider);
            _zipService = new ZipService();
            _dotnetZipService = new DotNetZipService();
            _excelService = new ExcelService();
        }

        public string SaveFileToBlob(Stream content, string name, string type, string subFolder = null)
        {
            return _fileServiceBlob.SaveAndReturnUri(content, name, type, subFolder);
        }

        public bool DeleteFileFromBlob(string name, string subFolder = null)
        {
            return _fileServiceBlob.Delete(name, subFolder);
        }

        public byte[] GetFileFromBlob(string name)
        {
            return _fileServiceBlob.GetFile(name).GetAwaiter().GetResult();
        }

        public byte[] ExcelListToStream<T>(List<T> list, bool isXlsx = true, string sheetName = "SheetOne")
        {
            return _excelService.ListToStream(list, isXlsx, sheetName);
        }

        public string ReadExcel(IFormFile file, int sheetIndex = 0)
        {
            return _excelService.Read(file, sheetIndex);
        }

        public async Task<byte[]> CompressionFilesReturnByte(List<CompressionFilesReturnByteArgs> p)
        {
            return await _zipService.CompressionFilesReturnByte(p);
        }

        public async Task<byte[]> CompressionFilesReturnByte(List<Poe_POEFile> files)
        {
            return await _dotnetZipService.CreateZipFile(files);
        }

    }
}
