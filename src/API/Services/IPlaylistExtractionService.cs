using AudioCloud.API.Data.Entities;

namespace AudioCloud.API.Services
{
    public interface IPlaylistExtractionService
    {
        Task<Playlist> ProcessPlaylistUpload(IFormFile archive);
    }
}
