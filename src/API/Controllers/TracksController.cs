using Microsoft.AspNetCore.Mvc;
using AudioCloud.Shared.DTOs;
using AudioCloud.API.Data;
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
        public ActionResult<CollectionResponseDto<string>> GetTrackNames(int? page = null, int? pageSize = null)
        {
            if (page.HasValue ^ pageSize.HasValue)
            {
                return BadRequest("Both page and pageSize must be provided.");
            }

            var titles = _context.Tracks
                .Select(t => t.Title)
                .Distinct()
                .ToList();

            if (!pageSize.HasValue)
            {
                return Ok(new CollectionResponseDto<string> { Data = titles, Info = new CollectionInfo{ Pagination = null } });
            }

            var pagedTitles = titles
                .Skip((page!.Value - 1) * pageSize.Value)
                .Take(pageSize.Value)
                .ToList();

            return Ok(new CollectionResponseDto<string>
            {
                Data = pagedTitles,
                Info = new CollectionInfo
                {
                    Pagination = new PaginationInfo
                    {
                        CurrentPage = page.Value,
                        PageSize = pagedTitles.Count,
                        TotalPages = (int)Math.Ceiling((double)titles.Count / pageSize.Value),
                        TotalItems = titles.Count
                    }
                }
            });
        }

        /// <summary>
        /// Gets tracks.
        /// </summary>
        /// <param name="search">The search term.</param>
        /// <returns>The list of tracks.</returns>
        [HttpGet]
        public ActionResult<CollectionResponseDto<TrackResponseDto>> GetTracks([FromQuery] string search)
        {
            _logger.LogInformation($"Searching for tracks with the term: {search}");

            var tracks = _context.Tracks
                .Where(t => t.Title.Contains(search))
                .Select(t => t.ToResponseDto(_urlOptions.PathPrefix))
                .ToList();
            
            var collection = new CollectionResponseDto<TrackResponseDto>
            {
                Data = tracks,
                Info = new CollectionInfo
                {
                    Title = search
                }
            };
            
            return Ok(collection);
        }
    }
}
