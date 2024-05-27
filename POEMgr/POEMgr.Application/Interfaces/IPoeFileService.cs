using FileService.Models;
using Microsoft.AspNetCore.Http;
using POEMgr.Repository.DbModels;

namespace POEMgr.Application.Interfaces
{
    public interface IPoeFileService
    {
        byte[] GetFileFromBlob(string name);
        string SaveFileToBlob(Stream content, string name, string type, string subFolder = null);
        bool DeleteFileFromBlob(string name, string subFolder = null);
        Task<byte[]> CompressionFilesReturnByte(List<CompressionFilesReturnByteArgs> p);
        Task<byte[]> CompressionFilesReturnByte(List<Poe_POEFile> files);
        string ReadExcel(IFormFile file, int sheetIndex = 0);
        byte[] ExcelListToStream<T>(List<T> list, bool isXlsx = true, string sheetName = "SheetOne");
    }
}
