namespace FilrConverter.Services.Implementation
{
    using FilrConverter.Services.Contacts;
    using FileConverter.Domain.Models;
    using Newtonsoft.Json;
    using System.Xml;
    using System.Text;
    using System.Collections.Generic;
    using System.IO.Compression;

    public class FileManager : IFileManager
    {
        private readonly IFileGuard _fileGuard;
        public FileManager(IFileGuard fileGuard)
        {
                _fileGuard = fileGuard;
        }


        /// <summary>
        /// Asynchronously archives a collection of files into a zip archive.
        /// </summary>
        /// <param name="files">An IEnumerable of InternalResult objects representing the files to be archived.</param>
        /// <returns>A Task containing a byte array representing the archived zip file.</returns>
        /// <remarks>
        /// This method creates a zip archive in memory using a MemoryStream and a ZipArchive.
        /// It iterates over each InternalResult object in the files collection.
        /// If the result indicates success, the processed file data is retrieved from the result.
        /// An entry is created in the zip archive with the processed file's FileName.
        /// The entry stream is opened and the processed file's data is copied to the entry stream asynchronously.
        /// Once all files have been processed, the zip archive is closed.
        /// The archived zip file is then returned as a byte array.
        /// </remarks>        
        public async Task<byte[]> ArchiveFilesAsync(IEnumerable<InternalResult<FileData>> files)
        {
            if (files is null || !files.Any())
            {
                return Array.Empty<byte>();
            }

            using var zipStream = new MemoryStream();
            
            using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {              
                foreach (var result in files)
                {
                    if (result.IsSuccess)
                    {
                        var processedFile = result.Data;
                        var entry = zipArchive.CreateEntry(processedFile.FileName);

                        using var entryStream = entry.Open();
                        await processedFile.Data.CopyToAsync(entryStream);
                    }
                }
            }

            // Set the position of the zip stream to the beginning
            zipStream.Position = 0;
            return zipStream.ToArray();
        }

        /// <summary>
        /// Asynchronously processes a file by converting XML data to JSON format.
        /// </summary>
        /// <param name="file">The FileData object representing the file to be processed.</param>
        /// <returns>A Task containing an InternalResult object of type FileData representing the result of the processing.</returns>
        /// <remarks>
        /// This method validates the file using the _fileGuard.Validate method. If the validation is successful, the file is processed further.
        /// The XML data is read using an XmlReader and loaded into an XmlDocument object.
        /// The XmlDocument is then converted to a JSON string using the JsonConvert.SerializeXmlNode method.
        /// The JSON string is converted to a byte array and stored in a MemoryStream.
        /// The file object is modified by setting its FileType property to ".json", replacing the file extension in the FileName property, and setting its Data property to the JSON stream.
        /// Finally, an InternalResult object is created with the modified file as the value and returned as the result of the method.
        /// If an exception occurs during the processing, an InternalResult object with an error message, status code 501, content type "text/xml", and the exception message is returned.
        /// </remarks>
        public async Task<InternalResult<FileData>> ProcessFileAsync(FileData file)
        {
            var validatedFile = _fileGuard.Validate(file);           

            if (validatedFile.IsSuccess)
            {
                try
                {
                    var result = new FileData();
                    using (var xmlReader = XmlReader.Create(file.Data))
                    {
                        var xmlDoc = new XmlDocument();
                        xmlDoc.Load(xmlReader);

                        string jsonString = JsonConvert.SerializeXmlNode(xmlDoc, Newtonsoft.Json.Formatting.None);

                        var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
                        var jsonStream = new MemoryStream(jsonBytes);
                        file.FileType = ".json";
                        file.FileName = file.FileName.Replace(".xml", ".json");
                        file.Data = jsonStream;
                        result = file;
                    }
                    return await Task.FromResult(new InternalResult<FileData>(result));
                }
                catch (Exception ex)
                {
                    return await Task.FromResult(new InternalResult<FileData>("Failed to process this xml file",501,"text/xml",ex.Message));

                }
            }

            return validatedFile;
        }
    }
}
