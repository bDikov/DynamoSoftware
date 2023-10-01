using System.IO;

namespace FileConverter.Domain.Models
{
    public class FileData
    {
        public string FileName { get; set; }

        public string FileType { get; set; }

        public int Length { get; set; }

        public Stream Data { get; set; }
    }
}
