using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kartowka.Core.Models.Configuration;

internal class RoundEntityTypeConfiguration : IEntityTypeConfiguration<Round>
{
    public void Configure(EntityTypeBuilder<Round> builder)
    {
        builder.HasIndex(round => round.Id);

        builder.Property<long>("PackId");

        builder
            .HasIndex("PackId", "Order")
            .IsUnique();

        builder
            .HasMany(round => round.Categories)
            .WithOne()
            .HasForeignKey("RoundId");
    }
}
