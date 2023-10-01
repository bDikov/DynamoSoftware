namespace FileConverter.Tests.FileConverter.Services.FileGuardTests
{
    using FilrConverter.Services.Implementation;
    using global::FileConverter.Domain.Models;
    using NUnit.Framework;

    [TestFixture]
    public class FileGuardTests
    {
        private FileGuard _fileGuard;

        [SetUp]
        public void Setup()
        {
            _fileGuard = new FileGuard();
        }

        [Test]
        public void Validate_WhenDataIsNull_ReturnsErrorResult()
        {
            // Arrange
            var fileData = new FileData
            {
                Data = null,
                FileType = "text/xml",
                Length = 100
            };

            // Act
            var result = _fileGuard.Validate(fileData);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("File can not be null or empty", result.Message);
            Assert.AreEqual(401, result.Code);
            Assert.AreEqual("text/xml", result.Type);
            Assert.AreEqual("File is null or empty", result.Errors.Single());
        }

        [Test]
        public void Validate_WhenFileTypeIsNotSupported_ReturnsErrorResult()
        {
            // Arrange
            var fileData = new FileData
            {
                Data = new MemoryStream(new byte[100]),
                FileType = "pdf",
                Length = 100
            };

            // Act
            var result = _fileGuard.Validate(fileData);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Unsupported file type", result.Message);
            Assert.AreEqual(402, result.Code);
            Assert.AreEqual("pdf", result.Type);
            Assert.AreEqual("File type must be xml", result.Errors.Single());
        }

        [Test]
        public void Validate_WhenFileSizeExceedsLimit_ReturnsErrorResult()
        {
            // Arrange
            var fileData = new FileData
            {
                Data = new MemoryStream(new byte[2000000]),
                FileType = "text/xml",
                Length = 2000000
            };

            // Act
            var result = _fileGuard.Validate(fileData);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("File size should not exceed 1MB", result.Message);
            Assert.AreEqual(403, result.Code);
            Assert.AreEqual("1,9073486328125", result.Type);
            Assert.AreEqual("File exceeds size limit", result.Errors.Single());
        }

        [Test]
        public void Validate_WhenDataIsValid_ReturnsSuccessResult()
        {
            // Arrange
            var fileData = new FileData
            {
                Data = new MemoryStream(new byte[100]),
                FileType = "text/xml",
                Length = 100
            };

            // Act
            var result = _fileGuard.Validate(fileData);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(fileData, result.Data);
            Assert.AreEqual(200, result.Code);
            Assert.IsNull(result.Message);
            Assert.IsEmpty(result.Errors);
        }
    }
}
