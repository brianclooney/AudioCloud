using AudioCloud.API.Data.Entities;
using AudioCloud.API.Models;
using AudioCloud.Shared.DTOs;

namespace AudioCloud.API.Extensions
{
    /// <summary>
    /// A class containing extension methods for the <see cref="Track"/> class.
    /// </summary>
    /// 
    public static class TrackExtensions
    {
        public static TrackResponseDto ToResponseDto(this Track track, string pathPrefix)
        {
            return new TrackResponseDto
            {
                Id = track.Id,
                OrdinalNumber = track.OrdinalNumber,
                Title = track.Title,
                Duration = track.Duration,
                Date = track.Date.ToString("yyyy-MM-dd"),
                Url = $"{pathPrefix}/{track.FilePath}"
            };
        }

        public static Track ToTrack(this ManifestTrack manifestTrack, DateTime dateRecorded, DateTime dateAdded)
        {
            return new Track
            {
                Title = manifestTrack.Title,
                Duration = manifestTrack.Duration,
                Date = dateRecorded,
                FilePath = manifestTrack.File,
                CreatedAt = dateAdded,
                OrdinalNumber = manifestTrack.Index
            };
        }
    }
}
