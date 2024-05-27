using Ionic.Zip;
using POEMgr.Repository.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileService
{
    public class DotNetZipService
    {
        public async Task<byte[]> CreateZipFile(List<Poe_POEFile> files)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var zip = new ZipFile(Encoding.Default);
            using (var webClient = new HttpClient())
            {
                foreach (var file in files)
                {
                    var uri = file.BlobUri;
                    zip.AddEntry(file.FileName, await webClient.GetStreamAsync(uri));
                }
            }

            using var ms = new MemoryStream();
            zip.Save(ms);
            return ms.ToArray();
        }
    }
}
