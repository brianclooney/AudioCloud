using AudioCloud.API.Configuration;
using Microsoft.Extensions.Options;

namespace AudioCloud.API.Services
{
    public class FileService : IFileService
    {
        private readonly FileServiceOptions _fileServiceOptions;
        private readonly ILogger<FileService> _logger;

        public FileService(IOptions<FileServiceOptions> fileServiceOptions, ILogger<FileService> logger)
        {
            _fileServiceOptions = fileServiceOptions.Value;
            _logger = logger;
        }

        public string GetRootPath()
        {
            return _fileServiceOptions.RootPath;
        }

        public string GetTempPath()
        {
            return _fileServiceOptions.TempPath;
        }

        public void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
    }
}
