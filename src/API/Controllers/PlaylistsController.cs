using AudioCloud.API.Data;
using AudioCloud.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistsController"/> class.
        /// </summary>
        /// <param name="context">An instance of <see cref="AudioCloudDbContext"/>.</param>
        public PlaylistsController(AudioCloudDbContext context)
        {
            _context = context;
        }

        /// <summary>
        ///  Gets playlist names.
        /// </summary>
        /// <returns>The playlist names with pagination.</returns>
        [HttpGet]
        public ActionResult<PaginationResponseDto<string>> GetPlaylistNames(int? page = null, int? pageSize = null)
        {
            if (page.HasValue ^ pageSize.HasValue)
            {
                return BadRequest("Both page and pageSize must be provided.");
            }

            var names = _context.Playlists
                .Select(p => p.Name)
                .ToList();

            if (!pageSize.HasValue)
            {
                return Ok(new PaginationResponseDto<string> { Data = names });
            }

            var pagedNames = names
                .Skip((page!.Value - 1) * pageSize.Value)
                .Take(pageSize.Value)
                .ToList();

            return Ok(new PaginationResponseDto<string>
            {
                Data = pagedNames,
                Pagination = new PaginationInfo
                {
                    CurrentPage = page.Value,
                    PageSize = pagedNames.Count,
                    TotalPages = (int)Math.Ceiling((double)names.Count / pageSize.Value),
                    TotalItems = names.Count
                }
            });
        }

        /// <summary>
        /// Gets all tracks for a playlist.
        /// </summary>
        /// <param name="playlistName">The name of the playlist.</param>
        /// <returns>The tracks for the playlist.</returns>
        [HttpGet("{playlistName}")]
        public ActionResult<List<TrackResponseDto>> GetTracks(string playlistName)
        {
            var tracks = _context.Tracks
                .Where(t => t.Playlist.Name == playlistName)
                .Select(t => new TrackResponseDto
                {
                    Id = t.Id,
                    OrdinalNumber = t.OrdinalNumber,
                    Title = t.Title,
                    Duration = t.Duration,
                    Date = t.Date.ToShortDateString(),
                    Url = $"/static/{t.FilePath}"
                })
                .ToList();

            return (tracks.Count == 0) ? NotFound() : Ok(tracks);
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
                .FirstOrDefault(p => p.Name == playlistName);

            if (playlist == null)
            {
                return NotFound();
            }

            _context.Playlists.Remove(playlist);

            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Upload tracks for a playlist contained in zip file.
        /// </summary>
        [HttpPost]
        public IActionResult UploadPlaylist()
        {
            var playlistName = "NewPlaylist";
            return CreatedAtAction(nameof(GetTracks), new { playlistName }, playlistName);
        }
    }

}
