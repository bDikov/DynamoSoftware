namespace FileConverter.Tests.FileConverter.Services
{
    using FilrConverter.Services.Contacts;
    using FilrConverter.Services.Implementation;
    using global::FileConverter.Domain.Models;
    using Moq;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Threading.Tasks;

    [TestFixture]
    public class ArchiveFilesAsyncTests
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
        public async Task ArchiveFilesAsync_WithValidFiles_ReturnsZipFile()
        {

            // Arrange
            var files = new List<InternalResult<FileData>>();

            var file = new FileData
            {
                FileType = "text/jsom",
                FileName = "file1.txt",
                Data = new MemoryStream(new byte[] { 1, 2, 3 })
            };

            var fileResponse = new InternalResult<FileData>(file, 200);

            var file2 = new FileData
            {
                FileType = "text/jsom",
                FileName = "file2.txt",
                Data = new MemoryStream(new byte[] { 4, 5, 6 })
            };

            var fileResponse2 = new InternalResult<FileData>(file2, 200);

            files.Add(fileResponse);
            files.Add(fileResponse2);

            var sut = _fileProcessor;

            // Act
            var result = await sut.ArchiveFilesAsync(files);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);

            using (var zipStream = new MemoryStream(result))
            using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read))
            {
                Assert.AreEqual(2, zipArchive.Entries.Count);
                Assert.IsTrue(zipArchive.Entries.Any(e => e.Name == "file1.txt"));
                Assert.IsTrue(zipArchive.Entries.Any(e => e.Name == "file2.txt"));
            }
        }

        [Test]
        public async Task ArchiveFilesAsync_WithEmptyFiles_ReturnsEmptyZipFile()
        {
            // Arrange
            var files = new List<InternalResult<FileData>>();

            var sut = _fileProcessor;

            // Act
            var result = await sut.ArchiveFilesAsync(files);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(Array.Empty<byte>().Length, result.Length);
        }

        [Test]
        public async Task ArchiveFilesAsync_WithInvalidFiles_ReturnsEmptyZipFile()
        {
            // Arrange
            var files = new List<InternalResult<FileData>>();

            var fileInternal = new InternalResult<FileData>(null, 400, "error", false);
            var fileInternal2 = new InternalResult<FileData>(null, 401, "error", false);

            files.Add(fileInternal);
            files.Add(fileInternal2);

            var sut = _fileProcessor; // Replace with the actual class containing the ArchiveFilesAsync method

            // Act
            var result = await sut.ArchiveFilesAsync(files);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(22, result.Length);
        }
    }
}