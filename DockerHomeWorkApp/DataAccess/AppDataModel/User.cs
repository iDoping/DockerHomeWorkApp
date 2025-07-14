using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DockerHomeWorkApp.DataAccess.AppDataModel;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public DateTime CreatedAt { get; set; }
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

		builder.Property(w => w.Username)
			.HasColumnName("username")
			.HasMaxLength(100)
			.IsRequired(true);

        builder.Property(w => w.Email)
            .HasColumnName("email")
            .HasMaxLength(100)
            .IsRequired(true);

        builder.Property(w => w.Email)
            .HasColumnName("email")
            .HasMaxLength(100)
            .IsRequired(true);

    }
}