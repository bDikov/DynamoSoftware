namespace FileConverter.Web.Api.Infrastructure.ValidatorStateAttributes
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;

    public class ValidFileCollectionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var files = value as IFormFileCollection;

            if (files == null || files.Count == 0)
            {
                return new ValidationResult("Please select at least one file.");
            }
            foreach (var file in files)
            {
                if (!file.ContentType.EndsWith("xml"))
                {
                    return new ValidationResult($"File Name: {file.FileName}\r\nFile type: {file.ContentType} is incorrect.\r\nPlease select only valid .xml files.");
                }
                if (file.Length > 1048576)
                {
                    return new ValidationResult($"File {file.Name}  size should not exceed 1MB");
                }
            }
            return ValidationResult.Success;
        }
    }

}
