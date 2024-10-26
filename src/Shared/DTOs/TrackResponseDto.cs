
namespace AudioCloud.Shared.DTOs
{
    public class TrackResponseDto
    {
        /// <summary>
        /// The unique identifier of the track.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The title of the track.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The URL where the track can be accessed.
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// The date when the track was created or added.
        /// </summary>
        public string Date { get; set; } = string.Empty;

        /// <summary>
        /// The duration of the track.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// The position of the track in a playlist.
        /// </summary>
        public int OrdinalNumber { get; set; }
    }
}
