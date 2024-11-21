using Microsoft.AspNetCore.Mvc;
using AudioCloud.Shared.DTOs;
using AudioCloud.API.Data;
using AudioCloud.API.Data.Entities;
using AudioCloud.API.Extensions;
using Microsoft.Extensions.Options;
using AudioCloud.API.Configuration;

namespace AudioCloud.API.Controllers
{
    /// <summary>
    /// A controller for managing tracks.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TracksController : ControllerBase
    {
        private readonly AudioCloudDbContext _context;
        private readonly ILogger<TracksController> _logger;
        private readonly UrlOptions _urlOptions;
        
        /// <summary>
        ///  Initializes a new instance of the <see cref="TracksController"/> class.
        /// </summary>
        /// <param name="context">An instance of <see cref="AudioCloudDbContext"/>.</param>
        /// <param name="logger">An instance of <see cref="ILogger{TracksController}"/>.</param>
        /// <param name="urlOptions">An instance of <see cref="IOptions{UrlOptions}"/>.</param>
        public TracksController(AudioCloudDbContext context, ILogger<TracksController> logger, IOptions<UrlOptions> urlOptions)
        {   
            _context = context;
            _logger = logger;
            _urlOptions = urlOptions.Value;
        }

        /// <summary>
        /// Gets track titles.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>The track titles with pagination.</returns>
        [HttpGet("names")]
        public ActionResult<IList<string>> GetTrackNames()
        {
            var titles = _context.Tracks
                .Select(t => t.Title)
                .Distinct()
                .ToList();

            return Ok(titles);
        }

        /// <summary>
        /// Gets tracks.
        /// </summary>
        /// <param name="search">The search term.</param>
        /// <returns>The list of tracks.</returns>
        [HttpGet]
        public ActionResult<CollectionResponseDto<TrackResponseDto>> GetTracks(
            string search, int? page = null, int? pageSize = null, string? orderBy = "title", string? order = "desc")
        {
            _logger.LogInformation($"Searching for tracks with the term: {search}");

            if (page.HasValue ^ pageSize.HasValue)
            {
                return BadRequest("Both page and pageSize must be provided.");
            }

            // Validate 'orderBy' parameter
            if (orderBy != "title" && orderBy != "date")
            {
                return BadRequest("Invalid orderBy parameter. Must be 'name'.");
            }

            // Validate 'order' parameter
            if (order != "asc" && order != "desc")
            {
                return BadRequest("Invalid order parameter. Must be 'asc' or 'desc'.");
            }

            // Initialize the query
            IQueryable<Track> query = _context.Tracks;

            // Apply ordering
            if (orderBy == "title")
            {
                query = order == "asc" ? query.OrderBy(p => p.Title) : query.OrderByDescending(p => p.Title);
            }
            else
            {
                query = order == "asc" ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt);
            }

            var tracks = _context.Tracks
                .Where(t => t.Title.Contains(search))
                .Select(t => t.ToResponseDto($"{_urlOptions.BaseAddress}{_urlOptions.PathPrefix}"))
                .ToList();

            if (!pageSize.HasValue)
            {
                return Ok(new CollectionResponseDto<TrackResponseDto> { Data = tracks, Info = new CollectionInfo { Title = search } });
            }

            var pagedTracks = tracks
                .Skip((page!.Value - 1) * pageSize.Value)
                .Take(pageSize.Value)
                .ToList();

            return Ok(new CollectionResponseDto<TrackResponseDto>
            {
                Data = pagedTracks,
                Info = new CollectionInfo
                {
                    Title = search,
                    Pagination = new PaginationInfo
                    {
                        CurrentPage = page.Value,
                        PageSize = pagedTracks.Count,
                        TotalPages = (int)Math.Ceiling((double)tracks.Count / pageSize.Value),
                        TotalItems = tracks.Count
                    }
                }
            });
        }
    }
}
