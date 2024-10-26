
namespace AudioCloud.API.Data.Entities
{
    public class Playlist
    {
        /// <summary>
        /// The unique identifier of the playlist.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the playlist.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Notes about the playlist.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// The date the playlist was added to the cloud.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Navigation property for the tracks in the playlist.
        /// </summary>
        public List<Track> Tracks { get; set; } = new List<Track>();
    }
}
