using AudioCloud.API.Controllers;
using AudioCloud.API.Data;
using AudioCloud.API.Data.Entities;
using AudioCloud.API.Tests.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using AudioCloud.Shared.DTOs;

namespace AudioCloud.API.Tests.Controllers
{
    /// <summary>
    /// Unit tests for the <see cref="PlaylistsController"/> class.
    /// </summary>
    public class PlaylistsControllerTests
    {
        private readonly AudioCloudDbContext _context = null!;
        private readonly PlaylistsController _controller;
        private readonly int _playlistCount = 7;
        private readonly int _tracksPerPlaylistCount = 14;

        public PlaylistsControllerTests()
        {
            var options = new DbContextOptionsBuilder<AudioCloudDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_Playlists")
                .Options;

            _context = new AudioCloudDbContext(options);
            _controller = new PlaylistsController(_context);
        }

        [Fact]
        public void GetPlaylistNames_ShouldReturnPaginatedPlaylistNames_WhenPaginationQueryProvided()
        {
            // Arrange
            TestDatabaseSeeder.SeedDatabase(_context, _playlistCount, _tracksPerPlaylistCount);

            // Act
            var result = _controller.GetPlaylistNames(1, 5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<PaginationResponseDto<string>>(okResult.Value);
            Assert.Equal(5, response.Data.Count);
            Assert.NotNull(response.Pagination);
            Assert.Equal(1, response.Pagination.CurrentPage);
            Assert.Equal(5, response.Pagination.PageSize);
            Assert.Equal(2, response.Pagination.TotalPages);
            Assert.Equal(_playlistCount, response.Pagination.TotalItems);
        }

        [Fact]
        public void GetPlaylistNames_ShouldReturnAllPlaylistNames_WhenPaginationQueryNotProvided()
        {
            // Arrange
            TestDatabaseSeeder.SeedDatabase(_context, _playlistCount, _tracksPerPlaylistCount);

            // Act
            var result = _controller.GetPlaylistNames();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<PaginationResponseDto<string>>(okResult.Value);
            Assert.Equal(_playlistCount, response.Data.Count);
            Assert.Null(response.Pagination);
        }

        [Fact]
        public void GetTracks_ShouldReturn404_WhenPlaylistNameNotFound()
        {
            // Arrange
            var playlistName = "NonExistentPlaylist";
            TestDatabaseSeeder.SeedDatabase(_context, _playlistCount, _tracksPerPlaylistCount);

            // Act
            var result = _controller.GetTracks(playlistName);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void GetTracks_ShouldReturnTracksForPlaylist_WhenPlaylistNameProvided()
        {
            // Arrange
            var playlistName = "Playlist3";
            TestDatabaseSeeder.SeedDatabase(_context, _playlistCount, _tracksPerPlaylistCount);

            // Act
            var result = _controller.GetTracks(playlistName);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<List<TrackResponseDto>>(okResult.Value);
            Assert.Equal(_tracksPerPlaylistCount, response.Count);
        }

        [Fact]
        public void DeletePlaylist_ShouldReturn404_WhenPlaylistNotFound()
        {
            // Arrange
            var playlistName = "NonExistentPlaylist";
            TestDatabaseSeeder.SeedDatabase(_context, _playlistCount, _tracksPerPlaylistCount);

            // Act
            var result = _controller.DeletePlaylist(playlistName);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeletePlaylist_ShouldReturn204_WhenPlaylistDeleted()
        {
            // Arrange
            var playlistName = "Playlist3";
            TestDatabaseSeeder.SeedDatabase(_context, _playlistCount+1, _tracksPerPlaylistCount);

            // Act
            var result = _controller.DeletePlaylist(playlistName);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Equal(_playlistCount*_tracksPerPlaylistCount, _context.Tracks.Count());
        }
    }
    
}