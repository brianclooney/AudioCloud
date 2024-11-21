using AudioCloud.API.Configuration;
using AudioCloud.API.Controllers;
using AudioCloud.API.Data;
using AudioCloud.API.Data.Entities;
using AudioCloud.API.Tests.Data;
using AudioCloud.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace AudioCloud.API.Tests.Controllers
{
    /// <summary>
    /// Unit tests for the <see cref="TracksController"/> class.
    /// </summary>
    public class TracksControllerTests
    {
        private readonly AudioCloudDbContext _context = null!;
        private readonly TracksController _controller;
        private readonly int _playlistCount = 7;
        private readonly int _tracksPerPlaylistCount = 14;
        private readonly UrlOptions _urlOptions = new UrlOptions { PathPrefix = "/static" };

        public TracksControllerTests()
        {
            var loggerMock = new Mock<ILogger<TracksController>>();
            var urlOptionsMock = new Mock<IOptions<UrlOptions>>();

            urlOptionsMock.SetupGet(o => o.Value).Returns(_urlOptions);

            var options = new DbContextOptionsBuilder<AudioCloudDbContext>()
                .UseInMemoryDatabase("TestDatabase_Tracks")
                .Options;

            _context = new AudioCloudDbContext(options);
            _controller = new TracksController(_context, loggerMock.Object, urlOptionsMock.Object);
        }

        [Fact]
        public void GetTrackTitles_ShouldReturnAllTrackNames()
        {
            // Arrange
            TestDatabaseSeeder.SeedDatabase(_context, _playlistCount, _tracksPerPlaylistCount);

            // Act
            var result = _controller.GetTrackNames();
        
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<List<string>>(okResult.Value);
            Assert.Equal(_tracksPerPlaylistCount, response.Count);
        }

        [Fact]
        public void GetTracks_ShouldReturnTracks_WhenFullNameMatches() 
        {
            // Arrange
            TestDatabaseSeeder.SeedDatabase(_context, _playlistCount, _tracksPerPlaylistCount);

            // Act
            var result = _controller.GetTracks("Track3");
        
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<CollectionResponseDto<TrackResponseDto>>(okResult.Value);
            Assert.Equal(_playlistCount, response.Data.Count);
        }

        [Fact]
        public void GetTracks_ShouldReturnTracks_WhenPartialNameMatches() 
        {
            // Arrange
            TestDatabaseSeeder.SeedDatabase(_context, _playlistCount, _tracksPerPlaylistCount);

            // Act
            var result = _controller.GetTracks("Track");
        
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<CollectionResponseDto<TrackResponseDto>>(okResult.Value);
            Assert.Equal(_playlistCount * _tracksPerPlaylistCount, response.Data.Count);
        }

        [Fact]
        public void GetTracks_ShouldReturnEmptyList_WhenNoTracksMatchGivenName() 
        {
            // Arrange
            TestDatabaseSeeder.SeedDatabase(_context, _playlistCount, _tracksPerPlaylistCount);

            // Act
            var result = _controller.GetTracks("NonExistentTrack");
        
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<CollectionResponseDto<TrackResponseDto>>(okResult.Value);
            Assert.Empty(response.Data);
        }
    }
}
