namespace DockerHomeWorkApp.DataContext;

using DockerHomeWorkApp.DataAccess.AppDataModel;
using Microsoft.EntityFrameworkCore;

public class AppDataContext(DbContextOptions<AppDataContext> options) : DbContext(options)
{
    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.HasDefaultSchema("public");
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}