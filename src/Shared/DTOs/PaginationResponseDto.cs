
namespace AudioCloud.Shared.DTOs
{
    /// <summary>
    ///  Represents a generic pagination DTO.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginationResponseDto<T>
    {
        /// <summary>
        /// Data for the current page.
        /// </summary>
        public List<T> Data { get; set; } = new List<T>();

        /// <summary>
        /// Pagination metadata for the current page.
        /// </summary>
        public PaginationInfo? Pagination { get; set; }
    }

    /// <summary>
    /// Represents pagination metadata.
    /// </summary>
    public class PaginationInfo
    {
        /// <summary>
        /// The current page number.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// The number of items per page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The total number of pages. 
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// The total number of items across all pages.
        /// </summary>
        public int TotalItems { get; set; }
    }
}
