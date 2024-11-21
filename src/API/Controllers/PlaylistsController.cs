using AudioCloud.API.Configuration;
using AudioCloud.API.Data;
using AudioCloud.API.Data.Entities;
using AudioCloud.API.Extensions;
using AudioCloud.API.Services;
using AudioCloud.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AudioCloud.API.Controllers
{
    /// <summary>
    /// A controller for managing playlists.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistsController : ControllerBase
    {
        private readonly AudioCloudDbContext _context;
        private readonly ILogger<PlaylistsController> _logger;
        private readonly IPlaylistExtractionService _playlistExtractionService;
        private readonly IFileService _fileService;
        private readonly UrlOptions _urlOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistsController"/> class.
        /// </summary>
        /// <param name="context">An instance of <see cref="AudioCloudDbContext"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{PlaylistsController}"/>.</param>
        /// <param name="playlistExtractionService">An instance of <see cref="IPlaylistExtractionService"/>.</param>
        /// <param name="fileService">An instance of <see cref="IFileService"/>.</param>
        /// <param name="urlOptions">An instance of <see cref="IOptions{UrlOptions}"/>.</param>
        public PlaylistsController(
            AudioCloudDbContext context, 
            ILogger<PlaylistsController> logger, 
            IPlaylistExtractionService playlistExtractionService,
            IFileService fileService,
            IOptions<UrlOptions> urlOptions)
        {
            _context = context;
            _logger = logger;
            _playlistExtractionService = playlistExtractionService;
            _fileService = fileService;
            _urlOptions = urlOptions.Value;
        }

        /// <summary>
        ///  Gets playlist names.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="orderBy">The field to order by.</param>
        /// <param name="order">The order of the results.</param>
        /// <returns>The playlist names with pagination.</returns>
        [HttpGet]
        public ActionResult<CollectionResponseDto<string>> GetPlaylistNames(int? page = null, int? pageSize = null, string? orderBy = "name", string? order = "desc")
        {
            if (page.HasValue ^ pageSize.HasValue)
            {
                return BadRequest("Both page and pageSize must be provided.");
            }

            // Validate 'orderBy' parameter
            if (orderBy != "name" && orderBy != "date")
            {
                return BadRequest("Invalid orderBy parameter. Must be 'name'.");
            }

            // Validate 'order' parameter
            if (order != "asc" && order != "desc")
            {
                return BadRequest("Invalid order parameter. Must be 'asc' or 'desc'.");
            }

            // Initialize the query
            IQueryable<Playlist> query = _context.Playlists;

            // Apply ordering
            if (orderBy == "name")
            {
                query = order == "asc" ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name);
            }
            else
            {
                query = order == "asc" ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt);
            }

            var names = query
                .Select(p => p.Name)
                .ToList();

            if (!pageSize.HasValue)
            {
                return Ok(new CollectionResponseDto<string> { Data = names });
            }

            var pagedNames = names
                .Skip((page!.Value - 1) * pageSize.Value)
                .Take(pageSize.Value)
                .ToList();

            return Ok(new CollectionResponseDto<string>
            {
                Data = pagedNames,
                Info = new CollectionInfo
                {
                    Pagination = new PaginationInfo
                    {
                        CurrentPage = page.Value,
                        PageSize = pagedNames.Count,
                        TotalPages = (int)Math.Ceiling((double)names.Count / pageSize.Value),
                        TotalItems = names.Count
                    }
                }
            });
        }

        /// <summary>
        /// Gets all tracks for a playlist.
        /// </summary>
        /// <param name="playlistName">The name of the playlist.</param>
        /// <returns>The tracks for the playlist.</returns>
        [HttpGet("{playlistName}")]
        public ActionResult<CollectionResponseDto<TrackResponseDto>> GetTracks(string playlistName)
        {
            var playlist = _context.Playlists
                .Include(p => p.Tracks)
                .FirstOrDefault(p => p.Name == playlistName);

            if (playlist == null)
            {
                return NotFound();
            }

            var collection = new CollectionResponseDto<TrackResponseDto>
            {
                Data = playlist.Tracks.Select(t => t.ToResponseDto($"{_urlOptions.BaseAddress}{_urlOptions.PathPrefix}")).ToList(),
                Info = new CollectionInfo
                {
                    Title = playlist.Name,
                    Description = playlist.Notes,
                    Date = playlist.CreatedAt.ToString("yyyy-MM-dd")
                }
            };

            return Ok(collection);
        }

        /// <summary>
        ///  Deletes a playlist and all its tracks.
        /// </summary>
        /// <param name="playlistName">The name of the playlist.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{playlistName}")]
        public IActionResult DeletePlaylist(string playlistName)
        {
            var playlist = _context.Playlists
                .Include(p => p.Tracks)
                .FirstOrDefault(p => p.Name == playlistName);

            if (playlist == null)
            {
                return NotFound();
            }

            playlist.Tracks.ForEach(t => _fileService.DeleteFile(t.FilePath));

            _context.Playlists.Remove(playlist);

            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Upload tracks for a playlist contained in zip file.
        /// </summary>
        /// <param name="archive">The zip file containing the tracks.</param>
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 104857600)] // 100 MB
        [RequestSizeLimit(104857600)] // 100 MB
        public async Task<IActionResult> UploadPlaylist(IFormFile archive)
        {
            _logger.LogInformation("PostRecordings");
            
	        try
            {
                var playlist = await _playlistExtractionService.ProcessPlaylistUpload(archive);
                _context.Playlists.Add(playlist);
		        _context.SaveChanges();
                // return Ok();
		        return CreatedAtAction(nameof(GetTracks), new { playlist.Name }, playlist.Name);
	        }
	        catch (ArgumentException e)
            {
                _logger.LogError("PostRecordings: " + e.Message);
	            return BadRequest(new { e.Message });
            }
        }   
    }
}
