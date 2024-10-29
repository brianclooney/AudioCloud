
namespace AudioCloud.API.Models
{
    public class Manifest
    {
        /// <summary>
        /// The title of the playlist.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The date the playlist was recorded.
        /// </summary>
        public DateTime DateRecorded { get; set; }

        /// <summary>
        /// The tracks in the playlist.
        /// </summary>
        public List<ManifestTrack> Tracks { get; set; } = new List<ManifestTrack>();
    }
}
