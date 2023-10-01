namespace FileConverter.Tests.FileConverter.Infrastructure
{
    using NUnit.Framework;
    using AutoMapper;
    using Microsoft.AspNetCore.Http;
    using global::FileConverter.Domain.Models;
    using global::FileConverter.Web.Api.Infrastructure;

    [TestFixture]
    public class MappingProfileTests
    {
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = configuration.CreateMapper();
        }

        [Test]
        public void CreateMap_WhenMappingIFormFileToFileData_ConfiguresMappingsCorrectly()
        {
            // Arrange
            var formFile = new FormFile(Stream.Null, 0, 0, "testFile", "testFile.txt")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };


            // Act
            var fileData = _mapper.Map<FileData>(formFile);

            // Assert
            Assert.AreEqual("testFile.txt", fileData.FileName);
            Assert.AreEqual("text/plain", fileData.FileType);
            Assert.AreEqual(0, fileData.Length);
            Assert.AreEqual(Stream.Null, fileData.Data);
        }
    }

}
