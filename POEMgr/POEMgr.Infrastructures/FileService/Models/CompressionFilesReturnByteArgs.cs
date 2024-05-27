using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Models
{
    public class CompressionFilesReturnByteArgs
    {
        public string Name { get; set; }

        public byte[] Stream { get; set; }
    }
}
