using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Profiles.Create.Infrastructure.Entities;
using Portfolio.Api.Profiles.Shared.Infrastructure.Entities;

namespace Portfolio.Api.Shared.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<ProfileEntity> Profiles { get; set; }
        public DbSet<ProfileTranslationEntity> ProfileTranslations { get; set; }
        public DbSet<LanguageEntity> Languages { get; set; }
    }
}