using Accounts.Domain.Users;
using Accounts.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounts.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("email")
            .HasConversion(
                v => v.Value,
                v => Email.FromTrustedSource(v)
            );

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("passwordhash")
            .HasConversion(
                v => v.Value,
                v => PasswordHash.FromHashed(v)
            );
        
        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity(j => j.ToTable("userroles"));
    }
}