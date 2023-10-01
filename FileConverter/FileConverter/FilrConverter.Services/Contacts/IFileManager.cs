namespace FilrConverter.Services.Contacts
{
    using FileConverter.Domain.Models;

    public interface IFileManager
    {
        Task<InternalResult<FileData>> ProcessFileAsync(FileData file);

        Task<byte[]> ArchiveFilesAsync(IEnumerable<InternalResult<FileData>> files);
    }
}
