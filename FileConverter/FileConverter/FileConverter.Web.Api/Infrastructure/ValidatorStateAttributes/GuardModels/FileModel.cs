namespace FileConverter.Web.Api.Infrastructure.ValidatorStateAttributes.GuardModels
{
    public class FileModel
    {
        [ValidFileCollection(ErrorMessage = "Please select at least one file.")]
        public IFormFileCollection Files { get; set; }
    }
}
