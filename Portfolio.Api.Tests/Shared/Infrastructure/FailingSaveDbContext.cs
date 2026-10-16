using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Shared.Infrastructure.Data;

namespace Portfolio.Api.Tests.Shared.Infrastructure;

public class FailingSaveDbContext(DbContextOptions<AppDbContext> options) : AppDbContext(options)
{
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException("Save failed");
}
