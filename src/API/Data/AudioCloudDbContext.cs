using AudioCloud.API.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AudioCloud.API.Data
{
    public class AudioCloudDbContext : DbContext
    {
        public DbSet<Playlist> Playlists { get; set; } = null!;
        public DbSet<Track> Tracks { get; set; } = null!;

        public AudioCloudDbContext(DbContextOptions<AudioCloudDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Playlist>()
                .HasMany(p => p.Tracks)
                .WithOne(t => t.Playlist)
                .HasForeignKey(t => t.PlaylistId);

            modelBuilder.Entity<Playlist>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
