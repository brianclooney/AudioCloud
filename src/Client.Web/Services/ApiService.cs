
using AudioCloud.Shared.DTOs;

namespace AudioCloud.Client.Web.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService> _logger;

        public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<CollectionResponseDto<string>> GetRecordingDatesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<CollectionResponseDto<string>>("/api/playlists") ?? new CollectionResponseDto<string>();
            }
            catch (Exception e)
            {
                _logger.LogError($"GetRecordingDatesAsync {e.Message}");
                return new CollectionResponseDto<string>();
            }
        }

        public async Task<CollectionResponseDto<TrackResponseDto>> GetRecordingsByDate(string date)
        {
            try
            {
                return await GetRecordings($"/api/playlists/{date}");
            }
            catch (Exception e)
            {
                _logger.LogError($"GetRecordingsByDate {e.Message}");
                return new CollectionResponseDto<TrackResponseDto>();
            }
        }

        public async Task<CollectionResponseDto<TrackResponseDto>> GetRecordingsByTitle(string searchString)
        {
            try
            {
                return await GetRecordings($"/api/recording?title={searchString}");
            }
            catch (Exception e)
            {
                _logger.LogError($"GetRecordingsByTitle {e.Message}");
                return new CollectionResponseDto<TrackResponseDto>();
            }
        }

        private async Task<CollectionResponseDto<TrackResponseDto>> GetRecordings(string uri)
        {
            try
            {
                var tracks = await _httpClient.GetFromJsonAsync<CollectionResponseDto<TrackResponseDto>>(uri) ?? new CollectionResponseDto<TrackResponseDto>();
                // tracks.Data.ForEach(t => t.Url = new Uri(_httpClient.BaseAddress!, t.Url).ToString());
                // tracks.ForEach(t => t.Url = new Uri(t.Url).ToString());
                return tracks;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetRecordings {e.Message}");
                return new CollectionResponseDto<TrackResponseDto>();
            }
        }
    }
}
