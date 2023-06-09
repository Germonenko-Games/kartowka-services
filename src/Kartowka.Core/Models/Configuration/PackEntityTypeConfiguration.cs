﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kartowka.Core.Models.Configuration;

public class PackEntityTypeConfiguration : IEntityTypeConfiguration<Pack>
{
    private const string PackIdPropertyName = "PackId";

    public void Configure(EntityTypeBuilder<Pack> builder)
    {
        builder.HasKey(pack => pack.Id);

        builder.HasMany(pack => pack.Assets)
            .WithOne()
            .HasForeignKey(PackIdPropertyName)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(pack => pack.QuestionsCategories)
            .WithOne()
            .HasForeignKey(PackIdPropertyName)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(pack => pack.Questions)
            .WithOne()
            .HasForeignKey(PackIdPropertyName)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(pack => pack.Rounds)
            .WithOne()
            .HasForeignKey(PackIdPropertyName)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey("AuthorId");
    }
}