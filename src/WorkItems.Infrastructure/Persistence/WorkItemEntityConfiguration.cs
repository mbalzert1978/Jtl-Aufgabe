// <copyright file="WorkItemEntityConfiguration.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkItems.Application.Adapters;

namespace WorkItems.Infrastructure.Persistence;

/// <summary>
/// Configures the entity mapping for <see cref="WorkItemEntity"/>.
/// </summary>
internal sealed class WorkItemEntityConfiguration : IEntityTypeConfiguration<WorkItemEntity>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<WorkItemEntity> builder)
    {
        Debug.Assert(builder is not null, $"{nameof(builder)} must not be null.");

        builder.ToTable("WorkItems");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).IsRequired().ValueGeneratedNever();

        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);

        builder.Property(e => e.Description).IsRequired().HasMaxLength(4096);

        builder.Property(e => e.Status).IsRequired().HasMaxLength(50);

        builder.Property(e => e.Priority).IsRequired().HasMaxLength(50);

        builder.Property(e => e.AssigneeId).IsRequired();

        builder.Property(e => e.EstimatedHours).IsRequired();

        builder.Property(e => e.DueDate).IsRequired(false);

        builder.Property(e => e.CompletedAt).IsRequired(false);

        builder.Property(e => e.ParentTaskId).IsRequired(false);

        builder.OwnsMany(
            e => e.Tags,
            tagsBuilder =>
            {
                Debug.Assert(tagsBuilder is not null, $"{nameof(tagsBuilder)} must not be null.");

                tagsBuilder.ToTable("WorkItemTags");

                tagsBuilder.WithOwner().HasForeignKey("WorkItemId");

                tagsBuilder.Property(t => t.Name).IsRequired().HasColumnName("Name");

                tagsBuilder.HasKey("WorkItemId", nameof(WorkItemTag.Name));

                tagsBuilder.HasIndex(nameof(WorkItemTag.Name));
            }
        );
    }
}
