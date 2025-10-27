// <copyright file="UserEntityConfiguration.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Application.Adapters;

namespace Users.Infrastructure.Persistence;

/// <summary>
/// Configures the entity mapping for <see cref="UserEntity"/>.
/// </summary>
internal sealed class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        Debug.Assert(builder is not null, $"{nameof(builder)} must not be null.");

        builder.ToTable("Users");

        builder.HasKey(e => e.UserId);

        builder.Property(e => e.UserId).IsRequired().ValueGeneratedNever();

        builder.Property(e => e.Username).IsRequired().HasMaxLength(200);
    }
}
