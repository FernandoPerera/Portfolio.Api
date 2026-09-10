using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Profiles.Create.Infrastructure.Entities;

namespace Portfolio.Api.Shared.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<ProfileEntity> Profiles { get; set; }
    }
}