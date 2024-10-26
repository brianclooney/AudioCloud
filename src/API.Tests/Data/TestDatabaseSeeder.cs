using AudioCloud.API.Data;
using AudioCloud.API.Data.Entities;

namespace AudioCloud.API.Tests.Data
{
    /// <summary>
    /// Helper class for seeding test data in the database.
    /// </summary>
    public class TestDatabaseSeeder
    {
        /// <summary>
        /// Adds a playlist with the specified name and track count to the database.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="name">The name of the playlist.</param>
        /// <param name="_trackCount">The number of tracks in the playlist.</param>
        public static void AddPlaylist(AudioCloudDbContext context, string name, int _trackCount)
        {
            var playlistId = Guid.NewGuid();

            context.Playlists.Add(new Playlist { 
                Id = playlistId,
                Name = name
            });

            for (var j = 0; j < _trackCount; j++)
            {
                context.Tracks.Add(new Track
                {
                    OrdinalNumber = j + 1,
                    Title = $"Track{j + 1}",
                    Duration = 120 + j * 30,
                    PlaylistId = playlistId
                });
            }

            context.SaveChanges();
        }

        /// <summary>
        ///  Seeds the database with the specified number of playlists and tracks per playlist.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="playlistCount">The number of playlists to add.</param>
        /// <param name="tracksPerPlaylistCount">The number of tracks per playlist.</param>
        public static void SeedDatabase(AudioCloudDbContext context, int playlistCount, int tracksPerPlaylistCount)
        {
            context.Database.EnsureDeleted();
            for (int i = 0; i < playlistCount; i++)
            {
                AddPlaylist(context, $"Playlist{i + 1}", tracksPerPlaylistCount);
            }
        }
    }
}
