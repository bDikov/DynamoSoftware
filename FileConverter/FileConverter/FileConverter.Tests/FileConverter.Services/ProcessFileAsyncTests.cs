namespace FileConverter.Tests.FileConverter.Services
{
    using FilrConverter.Services.Contacts;
    using FilrConverter.Services.Implementation;
    using global::FileConverter.Domain.Models;
    using Moq;
    using NUnit.Framework;
    using System.Text;

    [TestFixture]
    public class ProcessFileAsyncTests
    {
        private FileManager _fileProcessor;
        private Mock<IFileGuard> _fileGuardMock;

        [SetUp]
        public void Setup()
        {
            _fileGuardMock = new Mock<IFileGuard>();
            _fileProcessor = new FileManager(_fileGuardMock.Object);
        }

        [Test]
        public async Task ProcessFileAsync_ValidFile_ReturnsInternalResultWithProcessedFile()
        {
            // Arrange
            var fileData = new FileData
            {
                FileName = "test.xml",
                FileType = ".xml",
                Data = new MemoryStream(Encoding.UTF8.GetBytes("<root><name>John Doe</name></root>"))
            };
            var validatedFile = new InternalResult<FileData>(fileData);
            _fileGuardMock.Setup(fg => fg.Validate(fileData)).Returns(validatedFile);

            // Act
            var result = await _fileProcessor.ProcessFileAsync(fileData);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(".json", result.Data.FileType);
            Assert.AreEqual("test.json", result.Data.FileName);
            Assert.IsNotNull(result.Data.Data);
        }

        [Test]
        public async Task ProcessFileAsync_InvalidFile_ReturnsValidationResult()
        {
            // Arrange
            var fileData = new FileData();
            var validationResult = new InternalResult<FileData>(fileData, 400, "Invalid file", false);

            _fileGuardMock.Setup(fg => fg.Validate(fileData)).Returns(validationResult);


            // Act
            var result = await _fileProcessor.ProcessFileAsync(fileData);

            // Assert
            Assert.NotNull(result.Message);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid file", result.Message);
        }

        [Test]
        public async Task ProcessFileAsync_ExceptionThrown_ReturnsInternalResultWithError()
        {
            // Arrange
            var fileData = new FileData
            {
                FileName = "test.xml",
                FileType = ".xml",
                Data = new MemoryStream(Encoding.UTF8.GetBytes("<root><name>John Doe</name></root>"))
            };
            var validatedFile = new InternalResult<FileData>(fileData, 501, "Failed to process this xml file", false);
            _fileGuardMock.Setup(fg => fg.Validate(fileData)).Returns(validatedFile);

            // Act
            var result = await _fileProcessor.ProcessFileAsync(fileData);

            // Assert
            Assert.IsNotNull(result.Message);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Failed to process this xml file", result.Message);
            Assert.AreEqual(501, result.Code);
        }
    }


}
