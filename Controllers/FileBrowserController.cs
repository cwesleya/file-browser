using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using FileBrowser.Constants;
using FileBrowser.Models;

namespace FileBrowser.Controllers
{
    /// <summary>
    /// FileBrowserController for handling file and directory operations.
    /// </summary>
    [ApiController]
    [Route(AppConstants.ApiRouteBase)]
    [Produces("application/json")]
    public class FileBrowserController : ControllerBase
    {
        private readonly string _homeDirectory;
        private readonly ILogger<FileBrowserController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBrowserController"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public FileBrowserController(IConfiguration configuration, ILogger<FileBrowserController> logger)
        {
            var configuredHomeDirectory = configuration[AppConstants.DefaultRootDirectoryKey] ?? "/app";
            _homeDirectory = ExpandHomeDirectory(configuredHomeDirectory);
            _logger = logger;
        }

        /// <summary>
        /// Expands the home directory path if it starts with '~'.
        /// </summary>
        /// <param name="path">The path to expand.</param>
        /// <returns>The expanded path.</returns>
        private string ExpandHomeDirectory(string path)
        {
            if (path.StartsWith("~"))
            {
                var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                path = Path.Combine(homeDirectory, path.TrimStart('~', '/'));
            }
            return path;
        }

        /// <summary>
        /// Browses the specified directory.
        /// </summary>
        /// <param name="path">The path of the directory to browse.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>The contents of the directory.</returns>
        [HttpGet(AppConstants.BrowseEndpoint)]
        public IActionResult Browse([FromQuery] string? path = "", [FromQuery] int page = 1, [FromQuery] int pageSize = AppConstants.DefaultPageSize)
        {
            var fullPath = Path.Combine(_homeDirectory, path ?? string.Empty);

            if (!Directory.Exists(fullPath))
            {
                return NotFound(AppConstants.DirectoryNotFound);
            }

            var directoriesQuery = Directory.EnumerateDirectories(fullPath).Select(d => new DirectoryItem(new DirectoryInfo(d).Name, d));
            var filesQuery = Directory.EnumerateFiles(fullPath).Select(f => new FileItem(new FileInfo(f).Name, f, new FileInfo(f).Length));

            if (pageSize > 0)
            {
                directoriesQuery = directoriesQuery.Skip((page - 1) * pageSize).Take(pageSize);
                filesQuery = filesQuery.Skip((page - 1) * pageSize).Take(pageSize);
            }

            var directories = directoriesQuery.ToList();
            var files = filesQuery.ToList();

            return Ok(new { directories, files });
        }

        /// <summary>
        /// Searches for files matching the query.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>The search results.</returns>
        [HttpGet(AppConstants.SearchEndpoint)]
        public IActionResult Search([FromQuery] string? query = "", [FromQuery] int page = 1, [FromQuery] int pageSize = AppConstants.DefaultPageSize)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Browse(page: page, pageSize: pageSize);
            }

            var filesQuery = Directory.EnumerateFiles(_homeDirectory, $"*{query}*", SearchOption.AllDirectories)
                                      .Select(f => new FileItem(new FileInfo(f).Name, f, new FileInfo(f).Length));

            if (pageSize > 0)
            {
                filesQuery = filesQuery.Skip((page - 1) * pageSize).Take(pageSize);
            }

            var files = filesQuery.ToList();

            return Ok(files);
        }

        /// <summary>
        /// Uploads a file to the specified path.
        /// </summary>
        /// <param name="file">The file to upload.</param>
        /// <param name="path">The path to upload the file to.</param>
        /// <returns>The result of the upload operation.</returns>
        [HttpPost(AppConstants.UploadEndpoint)]
        public IActionResult Upload([FromForm] IFormFile file, [FromQuery] string? path = "")
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(AppConstants.NoFileUploaded);
            }

            var fullPath = Path.Combine(_homeDirectory, path ?? string.Empty, file.FileName);

            try
            {
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access to path: {Path}", fullPath);
                return StatusCode(403, AppConstants.UnauthorizedAccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error while uploading file to path: {Path}", fullPath);
                return StatusCode(500, AppConstants.InternalServerError + ex.Message);
            }

            return Ok(AppConstants.FileUploadedSuccessfully);
        }

        /// <summary>
        /// Deletes a file or directory.
        /// </summary>
        /// <param name="name">The name of the file or directory.</param>
        /// <param name="isDirectory">Whether the item is a directory.</param>
        /// <returns>The result of the delete operation.</returns>
        [HttpDelete(AppConstants.DeleteEndpoint)]
        public IActionResult Delete([FromQuery] string name, [FromQuery] bool isDirectory = false)
        {
            var fullPath = Path.Combine(_homeDirectory, name);

            if (isDirectory)
            {
                if (!Directory.Exists(fullPath))
                {
                    return NotFound(AppConstants.DirectoryNotFound);
                }

                Directory.Delete(fullPath, true);
                return Ok(AppConstants.DirectoryDeleted);
            }
            else
            {
                if (!System.IO.File.Exists(fullPath))
                {
                    return NotFound(AppConstants.FileNotFound);
                }

                System.IO.File.Delete(fullPath);
                return Ok(AppConstants.FileDeleted);
            }
        }

        /// <summary>
        /// Gets the home directory.
        /// </summary>
        /// <returns>The home directory path and its existence status.</returns>
        [HttpGet(AppConstants.HomeDirectoryEndpoint)]
        public IActionResult GetHomeDirectory()
        {
            var homeDirectoryExists = Directory.Exists(_homeDirectory);
            return Ok(new { path = _homeDirectory, exists = homeDirectoryExists });
        }
    }
}