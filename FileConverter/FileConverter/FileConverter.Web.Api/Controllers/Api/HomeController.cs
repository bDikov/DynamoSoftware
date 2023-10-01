using AutoMapper;
using FileConverter.Domain.Models;
using FileConverter.Web.Api.Infrastructure.ValidatorStateAttributes.GuardModels;
using FilrConverter.Services.Contacts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Concurrent;

namespace FileConverter.Web.Api.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IFileManager _fileManager;
        private readonly IMapper _mapper;
        public HomeController(IFileManager fileManager, IMapper mapper)
        {
            _fileManager = fileManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Uploads files and processes them.
        /// </summary>
        /// <param name="files">The files to upload.</param>
        /// <returns>The processed files as a zip file.</returns>
        
        [HttpPost]
        [SwaggerResponse(200, "Success")]
        [SwaggerOperation(Summary = "Uploads files and processes them")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type=typeof(FileModel))]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Upload([FromForm] FileModel files)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest( files);
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
            return File((await filesToReturn).ToArray(), "application/zip", "processed_files.zip");
        }
    }
}
