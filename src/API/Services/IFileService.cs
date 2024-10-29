
namespace AudioCloud.API.Services
{
    public interface IFileService
    {
        /// <summary>
        /// Deletes a file at the given path.
        /// </summary>
        /// <param name="path">The path of the file to delete (relative to the root).</param>
        void DeleteFile(string path);

        /// <summary>
        /// Deletes a directory at the given path.
        /// </summary>
        void DeleteDirectory(string path);

        /// <summary>
        /// Gets the root path for files.
        /// </summary>
        /// <returns>The root path for files.</returns>
        string GetRootPath();

        /// <summary>
        /// Gets the temporary path for files.
        /// </summary>
        /// <returns>The temporary path for files.</returns>
        string GetTempPath();
    }
}
