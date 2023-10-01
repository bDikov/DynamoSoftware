namespace FilrConverter.Services.Implementation
{
    using FileConverter.Domain.Models;
    using FilrConverter.Services.Contacts;

    public class FileGuard : IFileGuard
    {
        /// <summary>
        /// Validates the provided file data.
        /// </summary>
        /// <param name="data">The file data to be validated.</param>
        /// <returns>An InternalResult containing the validation result.</returns>
        /// This is dummy File Guard for example of additianal validation. In this case not in real use, but can be changed to work for specific XML validation like schema and so om..
        public InternalResult<FileData> Validate(FileData data)
        {
            if (data.Data == null || data.Data.Length == 0)
            {
                return new InternalResult<FileData>("File can not be null or empty",401,data.FileType,"File is null or empty");
            }
            if(!data.FileType.Equals("text/xml"))
            {
                return new InternalResult<FileData>("Unsupported file type", 402, data.FileType, "File type must be xml");
            }
            if (data.Length > 1048576)
            {
                return new InternalResult<FileData>("File size should not exceed 1MB", 403, ((double)data.Length / 1048576).ToString(), "File exceeds size limit");               
            }
            return new InternalResult<FileData>(data, 200);
        }
    }
}
