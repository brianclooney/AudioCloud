using Client.Web.Components;
using AudioCloud.Client.Web.Models;
using AudioCloud.Client.Web.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add serilog services to the container and read config from appsettings
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});
    
// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var baseAddress = builder.Configuration["ApiSettings:BaseAddress"] ?? "localhost";

Console.WriteLine($"API Base Address is {baseAddress}");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress)} );
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<RecordingsState>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
