using Microsoft.EntityFrameworkCore;

namespace Challenge.Infrastructure.Data.Persistence;

public class ChallengeDBContext : DbContext
{


    public ChallengeDBContext(DbContextOptions<ChallengeDBContext> contextOptions) : base(contextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


    }
}
