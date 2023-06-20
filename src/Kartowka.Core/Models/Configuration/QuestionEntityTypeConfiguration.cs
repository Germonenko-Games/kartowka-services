using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kartowka.Core.Models.Configuration;

public class QuestionEntityTypeConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasKey(question => question.Id);

        builder.Property<long>("PackId");

        builder.Property<long?>("AssetId");

        builder.HasOne(question => question.Asset)
            .WithOne()
            .HasForeignKey<Question>("AssetId")
            .OnDelete(DeleteBehavior.SetNull);
    }
}