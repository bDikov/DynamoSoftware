namespace FileConverter.Web.Api.Controllers
{
    using AutoMapper;
    using FileConverter.Domain.Models;
    using FileConverter.Web.Api.Infrastructure.ValidatorStateAttributes.GuardModels;
    using FilrConverter.Services.Contacts;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Concurrent;

    public class HomeController : Controller
    {
        private readonly IFileManager _fileManager;
        private readonly IMapper _mapper;
        public HomeController(IFileManager fileManager, IMapper mapper)
        {
            _fileManager = fileManager;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Handles the HTTP POST request for uploading a list of files.
        /// </summary>
        /// <param name="files">The list of files to be uploaded.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the success of the upload.</returns>

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] FileModel files)
        {
            if (!ModelState.IsValid)
            {                
                return View("InvalidSelectedDocuments", files);
            }
            var results = new ConcurrentBag<InternalResult<FileData>>();

            await Task.Run(async () =>
            {
                foreach (var file in files.Files)
                {
                    var fileData = _mapper.Map<FileData>(file);
                    results.Add(await _fileManager.ProcessFileAsync(fileData));
                }
            });

            var filesToReturn = _fileManager.ArchiveFilesAsync(results);
            return File( (await filesToReturn).ToArray(), "application/zip", "processed_files.zip");
        }
    }
}
