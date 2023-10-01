namespace FilrConverter.Services.Contacts
{
    using FileConverter.Domain.Models;

    public interface IFileGuard
    {
        InternalResult<FileData> Validate(FileData data);

    }
}
