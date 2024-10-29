
namespace AudioCloud.Shared.DTOs
{
    /// <summary>
    /// A class representing a collection response.
    /// </summary>
    /// <typeparam name="T">The type of the data in the collection.</typeparam>
    public class CollectionResponseDto<T>
    {
        /// <summary>
        /// The data in the collection.
        /// </summary>
        public List<T> Data { get; set; } = new List<T>();

        /// <summary>
        /// Information about the collection.
        /// </summary>
        public CollectionInfo Info { get; set; } = new CollectionInfo();
    }

    public class CollectionInfo
    {
        /// <summary>
        /// The title of the collection, if applicable.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// The description of the collection, if applicable.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The date when the collection was created, if applicable.
        /// </summary>
        public string? Date { get; set; }

        /// <summary>
        /// Pagination metadata for the current page, if applicable.
        /// </summary>
        public PaginationInfo? Pagination { get; set; }
    }
}
