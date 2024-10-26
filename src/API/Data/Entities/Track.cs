
using System.ComponentModel.DataAnnotations.Schema;

namespace AudioCloud.API.Data.Entities
{
    public class Track
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
        ///  Notes about the track.
        /// </summary>
        public string? Notes { get; set; } = string.Empty;

        /// <summary>
        ///  The file path where the track is stored.
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// The duration of the track in seconds.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// The date the track was released.
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// The position of the track in a playlist.
        /// </summary>
        public int OrdinalNumber { get; set; }

        /// <summary>
        /// The date when the track was added.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Navigation property for the playlist that the track belongs to.
        /// </summary>
        public Guid PlaylistId { get; set; }
        public Playlist Playlist { get; set; } = null!;
    }
}
