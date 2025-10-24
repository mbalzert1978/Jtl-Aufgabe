using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Application.Adapters;

namespace Users.Infrastructure.Persistence;

internal sealed class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(e => e.UserId);
        builder.Property(e => e.Username).IsRequired().HasMaxLength(200);
    }
}
