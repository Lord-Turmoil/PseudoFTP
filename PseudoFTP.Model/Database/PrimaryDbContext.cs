using Microsoft.EntityFrameworkCore;

namespace PseudoFTP.Model.Database;

public class PrimaryDbContext : DbContext
{
    public PrimaryDbContext(DbContextOptions<PrimaryDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<TransferHistory> TransferHistories { get; set; } = null!;
    public DbSet<Profile> Profiles { get; set; } = null!;
}