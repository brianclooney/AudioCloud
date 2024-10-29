using System.Text.Json;
using System.Text.Json.Serialization;
using AudioCloud.API.Configuration;
using AudioCloud.API.Data;
using AudioCloud.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FileServiceOptions>(builder.Configuration.GetSection("FileServiceOptions"));
builder.Services.Configure<UrlOptions>(builder.Configuration.GetSection("UrlOptions"));

// Set up Serilog
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger());

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AudioCloudDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("AudioCloudDatabase");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddScoped<IPlaylistExtractionService, PlaylistExtractionService>();
builder.Services.AddScoped<IFileService, FileService>();

var app = builder.Build();

var fileServiceOptions = app.Services.GetRequiredService<IOptions<FileServiceOptions>>().Value;
var urlOptions = app.Services.GetRequiredService<IOptions<UrlOptions>>().Value;

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(fileServiceOptions.RootPath),
    RequestPath = urlOptions.PathPrefix
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors(b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

#pragma warning disable ASP0014
app.UseEndpoints(e => e.MapControllers());
#pragma warning restore ASP0014

app.Run();
