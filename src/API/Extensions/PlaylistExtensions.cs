using AudioCloud.API.Data.Entities;
using AudioCloud.API.Models;

namespace AudioCloud.API.Extensions
{
    public static class PlaylistExtensions
    {
        public static Playlist ToPlaylist(this Manifest manifest)
        {
            var dateRecorded = manifest.DateRecorded;
            var dateAdded = DateTime.Now;

            return new Playlist
            {
                Name = manifest.Title,
                CreatedAt = manifest.DateRecorded,
                Tracks = manifest.Tracks.Select(t => t.ToTrack(dateRecorded, dateAdded)).ToList()
            };
        }
    }
}
