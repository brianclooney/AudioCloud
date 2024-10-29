
namespace AudioCloud.API.Models
{
    public class ManifestTrack
    {
        /// <summary>
        /// The index of the track in the playlist.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The file path of the track.
        /// </summary>
        public string File { get; set; } = string.Empty;

        /// <summary>
        /// The title of the track.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The duration of the track in seconds.
        /// </summary>
        public int Duration { get; set; }
    }
}
