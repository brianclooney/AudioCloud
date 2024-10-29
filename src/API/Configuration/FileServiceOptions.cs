
namespace AudioCloud.API.Configuration
{
    public class FileServiceOptions
    {
     	public string TempPath { get; set; } = "/app/tmp";
	    public string RootPath { get; set; } = "/app/static";
	    // public string UrlPathPrefix { get; set; } = "/static";
    }
}
