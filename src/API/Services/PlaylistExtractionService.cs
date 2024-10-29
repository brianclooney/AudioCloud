using System.IO.Compression;
using System.Text.Json;
using AudioCloud.API.Data.Entities;
using AudioCloud.API.Extensions;
using AudioCloud.API.Models;

namespace AudioCloud.API.Services
{
    public class PlaylistExtractionService : IPlaylistExtractionService
    {
        private readonly IFileService _fileService;

        public PlaylistExtractionService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<Playlist> ProcessPlaylistUpload(IFormFile archive)
        {
            // Ensure the uploaded file is a zip archive
            if (archive == null || !archive.FileName.EndsWith(".zip"))
            {
                throw new ArgumentException("A zip archive is required.");
            }

            var uploadId = Guid.NewGuid().ToString();

            // Define a path to temporarily store the uploaded archive
            var tempFilePath = $"{_fileService.GetTempPath()}/{uploadId}.tmp";

            // Save the uploaded archive to the tempFilePath
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await archive.CopyToAsync(stream);
            }

            // Define a directory to extract the archive to
            var extractionPath = $"{_fileService.GetRootPath()}/{uploadId}";

            // Extract the contents of the archive
            ZipFile.ExtractToDirectory(tempFilePath, extractionPath);

            // Clean up the temporary file
            File.Delete(tempFilePath);

            // Read the metadata file (manifest.json)
            var manifest = ReadManifest(extractionPath);

            if (manifest == null)
            {
                // Clean up the extracted files
                Directory.Delete(extractionPath, true);

                throw new ArgumentException("Unable to read manifest file.");
            }

            var id = Guid.NewGuid();
            // string targetDirectoryPath = $"{_fileService.GetRootPath()}/{manifest.DateRecorded:yyyy-MM-dd}";
            string targetDirectoryPath = Path.Combine(_fileService.GetRootPath(), id.ToString());
            if (Directory.Exists(targetDirectoryPath))
            {
                // Clean up the extracted files
                Directory.Delete(extractionPath, true);

                throw new ArgumentException("Files already exist for Guid.");
            }
            else
            {
                // Update the manifest with the static file root path
                Directory.Move(extractionPath, targetDirectoryPath);
            }

            // Update the manifest tracks with the static file request path
            foreach (var track in manifest.Tracks)
            {
                track.File = $"{id}/{track.File}";
            }

            var playlist = manifest.ToPlaylist();
            playlist.Id = id;
            return playlist;
        }

        private Manifest? ReadManifest(string extractionPath)
        {
            var manifestFilePath = Path.Combine(extractionPath, "manifest.json");
            var manifestJson = File.ReadAllText(manifestFilePath);
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return JsonSerializer.Deserialize<Manifest>(manifestJson, options);
        }
    }
}
