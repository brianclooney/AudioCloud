using AudioCloud.API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AudioCloudDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("AudioCloudDatabase");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

var app = builder.Build();

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
