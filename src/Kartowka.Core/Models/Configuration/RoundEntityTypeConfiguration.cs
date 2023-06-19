using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kartowka.Core.Models.Configuration;

public class RoundEntityTypeConfiguration : IEntityTypeConfiguration<Round>
{
    public void Configure(EntityTypeBuilder<Round> builder)
    {
        builder.HasKey(round => round.Id);

        builder.Property<long>("PackId");

        builder.HasMany<QuestionsCategory>()
            .WithOne()
            .HasForeignKey(category => category.RoundId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}