using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DockerHomeWorkApp.DataAccess.AppDataModel;

public class User
{
    public long Id { get; set; }
    public string Email { get; set; } = default!;
    public byte[] PasswordHash { get; set; } = default!;
    public byte[] PasswordSalt { get; set; } = default!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("users");

        builder.HasKey(x => x.Id)
            .HasName("users_pkey");

        builder.Property(w => w.Id)
            .HasColumnName("id")
            .UseIdentityAlwaysColumn();

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(320);

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.PasswordHash)
            .IsRequired();

        builder.Property(x => x.PasswordSalt)
            .IsRequired();

        builder.Property(x => x.FirstName)
            .HasMaxLength(200);

        builder.Property(x => x.LastName)
            .HasMaxLength(200);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("now()");

        builder.Property(x => x.UpdatedAt)
            .HasDefaultValueSql("now()");
    }
}